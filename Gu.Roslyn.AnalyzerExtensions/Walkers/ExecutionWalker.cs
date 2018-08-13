namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Walks code as it is executed.
    /// </summary>
    /// <typeparam name="T">The inheriting type.</typeparam>
    public abstract class ExecutionWalker<T> : PooledWalker<T>
        where T : ExecutionWalker<T>
    {
        private readonly HashSet<SyntaxNode> visited = new HashSet<SyntaxNode>();

        /// <summary>
        /// Gets or sets if the walker should walk declarations of invoked methods etc.
        /// </summary>
        protected Scope Scope { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SemanticModel"/>
        /// </summary>
        protected SemanticModel SemanticModel { get; set; }

        protected ITypeSymbol ContainingType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CancellationToken"/>
        /// </summary>
        protected CancellationToken CancellationToken { get; set; }

        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            this.VisitTypeDeclaration(node);
        }

        /// <inheritdoc />
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            this.VisitTypeDeclaration(node);
        }

        /// <inheritdoc />
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (this.Scope != Scope.Member &&
                node.Initializer == null &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var ctor) &&
                ctor.ContainingType is INamedTypeSymbol containingType &&
                Constructor.TryFindDefault(containingType.BaseType, Search.Recursive, out var defaultCtor) &&
                defaultCtor.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax defaultCtorDeclaration))
            {
                this.Visit(defaultCtorDeclaration);
            }

            base.VisitConstructorDeclaration(node);
        }

        /// <inheritdoc />
        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            base.VisitConstructorInitializer(node);
            switch (this.Scope)
            {
                case Scope.Member:
                    break;
                case Scope.Instance:
                case Scope.Type:
                case Scope.Recursive:
                    if (this.visited.Add(node) &&
                        node.TryGetTargetDeclaration(this.SemanticModel, this.CancellationToken, out var declaration))
                    {
                        this.Visit(declaration);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (this.Scope == Scope.Member)
            {
                base.VisitObjectCreationExpression(node);
                return;
            }

            if (this.visited.Add(node) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var target))
            {
                if (this.Scope.IsEither(Scope.Instance, Scope.Type) &&
                    !target.ContainingType.Equals(this.ContainingType))
                {
                    base.VisitObjectCreationExpression(node);
                    return;
                }

                if (target.ContainingType.TrySingleDeclaration(this.CancellationToken, out TypeDeclarationSyntax containingTypeDeclaration))
                {
                    using (var walker = TypeDeclarationWalker.Borrow(containingTypeDeclaration))
                    {
                        foreach (var initializer in walker.Initializers)
                        {
                            if (this.visited.Add(initializer))
                            {
                                this.Visit(initializer);
                            }
                        }
                    }
                }

                if (target.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax declaration))
                {
                    this.Visit(declaration);
                }

                base.VisitObjectCreationExpression(node);
            }
        }

        /// <inheritdoc />
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
            switch (this.Scope)
            {
                case Scope.Member:
                    break;
                case Scope.Instance:
                case Scope.Type:
                case Scope.Recursive:
                    if (TryGetTarget(out var target))
                    {
                        this.Visit(target);
                    }

                    break;
            }

            bool TryGetTarget(out MethodDeclarationSyntax declaration)
            {
                declaration = null;
                if (this.Scope == Scope.Instance &&
                    !MemberPath.IsEmpty(node))
                {
                    return false;
                }

                return this.visited.Add(node) &&
                       node.TryGetTargetDeclaration(this.SemanticModel, this.CancellationToken, out declaration);
            }
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);
            switch (this.Scope)
            {
                case Scope.Member:
                    break;
                case Scope.Instance:
                case Scope.Type:
                case Scope.Recursive:
                    if (this.TryGetPropertyGet(node, out var target))
                    {
                        this.Visit(target);
                    }

                    break;
            }
        }

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            switch (this.Scope)
            {
                case Scope.Member:
                    break;
                case Scope.Instance:
                case Scope.Type:
                case Scope.Recursive:
                    if (this.TryGetPropertyGet(node.Right, out var getter))
                    {
                        this.Visit(getter);
                    }

                    if (this.TryGetPropertySet(node.Left, out var setter))
                    {
                        this.Visit(setter);
                    }

                    break;
            }

            base.VisitAssignmentExpression(node);
        }

        /// <summary>
        /// Returns a walker that have visited <paramref name="node"/>
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="create">The factory for creating a walker if not found in cache.</param>
        /// <returns>The walker that have visited <paramref name="node"/>.</returns>
        protected static T BorrowAndVisit(SyntaxNode node, Scope scope, SemanticModel semanticModel, CancellationToken cancellationToken, Func<T> create)
        {
            var walker = Borrow(create);

            // Not pretty below here, throwing is perhaps nicer, dunno.
            walker.Scope = scope == Scope.Member &&
                           node is TypeDeclarationSyntax ? Scope.Type : scope;
            if (walker.Scope != Scope.Member)
            {
                if (node is TypeDeclarationSyntax typeDeclaration &&
                    semanticModel.TryGetSymbol(typeDeclaration, cancellationToken, out var containingType))
                {
                    walker.ContainingType = containingType;
                }
                else if (node.TryFirstAncestor(out TypeDeclarationSyntax containingTypeDeclaration) &&
                         semanticModel.TryGetSymbol(containingTypeDeclaration, cancellationToken, out containingType))
                {
                    walker.ContainingType = containingType;
                }
            }

            walker.SemanticModel = semanticModel;
            walker.CancellationToken = cancellationToken;
            walker.Visit(node);
            walker.Scope = scope;
            return walker;
        }

        /// <summary>
        /// Called by <see cref="VisitClassDeclaration"/> and <see cref="VisitStructDeclaration"/>
        /// Walks the members in the following order:
        /// 1. Field and property initializers in document order.
        /// 2. Nonprivate constructors in document order
        /// 3. Nonprivate members.
        /// 4. Nested types if scope is recursive.
        /// </summary>
        /// <param name="node">The <see cref="TypeDeclarationSyntax"/></param>
        protected virtual void VisitTypeDeclaration(TypeDeclarationSyntax node)
        {
            using (var walker = TypeDeclarationWalker.Borrow(node))
            {
                foreach (var initializer in walker.Initializers)
                {
                    this.Visit(initializer);
                }

                foreach (var ctor in walker.Ctors)
                {
                    if (this.Scope == Scope.Instance &&
                        ctor.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    this.Visit(ctor);
                }

                foreach (var member in walker.Members)
                {
                    this.Visit(member);
                }

                if (this.Scope == Scope.Recursive)
                {
                    foreach (var type in walker.Types)
                    {
                        this.Visit(type);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.visited.Clear();
            this.SemanticModel = null;
            this.ContainingType = null;
            this.CancellationToken = CancellationToken.None;
        }

        private bool TryGetPropertyGet(SyntaxNode candidate, out SyntaxNode getter)
        {
            getter = null;
            if (!this.visited.Add(candidate))
            {
                return false;
            }

            if (this.Scope == Scope.Instance &&
                candidate is MemberAccessExpressionSyntax memberAccess &&
                !(memberAccess.Expression is InstanceExpressionSyntax))
            {
                return false;
            }

            if (candidate.Parent is MemberAccessExpressionSyntax memberAccessParent &&
                memberAccessParent.Expression is InstanceExpressionSyntax)
            {
                return this.TryGetPropertyGet(memberAccessParent, out getter);
            }

            if (candidate.TryFirstAncestor<ArgumentSyntax>(out _) ||
                (candidate.TryFirstAncestor<AssignmentExpressionSyntax>(out var assignment) &&
                 assignment.Right.Contains(candidate)) ||
                candidate.Parent is ExpressionStatementSyntax ||
                candidate.TryFirstAncestor<EqualsValueClauseSyntax>(out _) ||
                candidate.TryFirstAncestor<ArrowExpressionClauseSyntax>(out _))
            {
                return this.SemanticModel.TryGetSymbol(candidate, this.CancellationToken, out IPropertySymbol property) &&
                       property.GetMethod is IMethodSymbol getMethod &&
                       getMethod.TrySingleDeclaration(this.CancellationToken, out getter);
            }

            return false;
        }

        private bool TryGetPropertySet(SyntaxNode candidate, out AccessorDeclarationSyntax setter)
        {
            setter = null;
            if (!this.visited.Add(candidate))
            {
                return false;
            }

            if (this.Scope == Scope.Instance &&
                candidate is MemberAccessExpressionSyntax memberAccess &&
                !(memberAccess.Expression is InstanceExpressionSyntax))
            {
                return false;
            }

            if (candidate.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Left.Contains(candidate))
            {
                return this.SemanticModel.TryGetSymbol(candidate, this.CancellationToken, out IPropertySymbol property) &&
                       property.SetMethod is IMethodSymbol setMethod &&
                       setMethod.TrySingleDeclaration(this.CancellationToken, out setter);
            }

            return false;
        }

        private class TypeDeclarationWalker : PooledWalker<TypeDeclarationWalker>
        {
#pragma warning disable SA1401 // Fields must be private
            internal readonly List<EqualsValueClauseSyntax> Initializers = new List<EqualsValueClauseSyntax>();
            internal readonly List<ConstructorDeclarationSyntax> Ctors = new List<ConstructorDeclarationSyntax>();
            internal readonly List<MemberDeclarationSyntax> Members = new List<MemberDeclarationSyntax>();
            internal readonly List<TypeDeclarationSyntax> Types = new List<TypeDeclarationSyntax>();
#pragma warning restore SA1401 // Fields must be private

            public static TypeDeclarationWalker Borrow(TypeDeclarationSyntax typeDeclaration)
            {
                var walker = Borrow(() => new TypeDeclarationWalker());
                foreach (var member in typeDeclaration.Members)
                {
                    walker.Visit(member);
                }

                return walker;
            }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (node.Declaration is VariableDeclarationSyntax declaration &&
                    declaration.Variables.TryLast(out var variable) &&
                    variable.Initializer is EqualsValueClauseSyntax equalsValueClause)
                {
                    this.Initializers.Add(equalsValueClause);
                }
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (node.Initializer is EqualsValueClauseSyntax equalsValueClause)
                {
                    this.Initializers.Add(equalsValueClause);
                }

                if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    this.Members.Add(node);
                }
            }

            public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    if (this.Ctors.Count > 0 &&
                        node.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        this.Ctors.Insert(0, node);
                    }
                    else
                    {
                        this.Ctors.Add(node);
                    }
                }
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    this.Members.Add(node);
                }
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                this.Types.Add(node);
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                this.Types.Add(node);
            }

            protected override void Clear()
            {
                this.Initializers.Clear();
                this.Ctors.Clear();
                this.Members.Clear();
                this.Types.Clear();
            }
        }
    }
}

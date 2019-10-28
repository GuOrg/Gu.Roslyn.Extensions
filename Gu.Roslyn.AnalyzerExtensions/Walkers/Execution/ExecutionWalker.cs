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
        protected SearchScope SearchScope { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SemanticModel"/>.
        /// </summary>
        protected SemanticModel SemanticModel { get; set; }

        /// <summary>
        /// Gets or sets the containing <see cref="ITypeSymbol"/> of the current context.
        /// </summary>
        protected ITypeSymbol ContainingType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CancellationToken"/>.
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
            if (this.SearchScope != SearchScope.Member &&
                node?.Initializer is null &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var ctor) &&
                ctor.ContainingType is INamedTypeSymbol containingType &&
                Constructor.TryFindDefault(containingType.BaseType, Search.Recursive, out var defaultCtor) &&
                defaultCtor.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? defaultCtorDeclaration))
            {
                this.Visit(defaultCtorDeclaration);
            }

            base.VisitConstructorDeclaration(node);
        }

        /// <inheritdoc />
        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            base.VisitConstructorInitializer(node);
            switch (this.SearchScope)
            {
                case SearchScope.Member:
                    break;
                case SearchScope.Instance:
                case SearchScope.Type:
                case SearchScope.Recursive:
                    if (this.visited.Add(node) &&
                        node.TryGetTargetDeclaration(this.SemanticModel, this.CancellationToken, out var declaration))
                    {
                        this.Visit(declaration);
                    }

                    break;
                default:
                    throw new InvalidOperationException($"Not handling member {this.SearchScope}.");
            }
        }

        /// <inheritdoc />
        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (this.SearchScope == SearchScope.Member)
            {
                base.VisitObjectCreationExpression(node);
                return;
            }

            if (this.visited.Add(node) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var target))
            {
                if (this.SearchScope.IsEither(SearchScope.Instance, SearchScope.Type) &&
                    !target.ContainingType.Equals(this.ContainingType))
                {
                    base.VisitObjectCreationExpression(node);
                    return;
                }

                if (target.ContainingType.TrySingleDeclaration(this.CancellationToken, out TypeDeclarationSyntax? containingTypeDeclaration))
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

                if (target.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? declaration))
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
            if (this.TryGetTargetSymbol(node, out IMethodSymbol target) &&
                target.TrySingleDeclaration(this.CancellationToken, out MethodDeclarationSyntax? declaration))
            {
                this.Visit(declaration);
            }
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);
            if (this.TryGetTargetSymbol(node, out IPropertySymbol property))
            {
                if (this.IsPropertyGetAndSet(node))
                {
                    if (property.GetMethod.TrySingleAccessorDeclaration(this.CancellationToken, out var getter))
                    {
                        this.Visit(getter);
                    }

                    if (property.SetMethod.TrySingleAccessorDeclaration(this.CancellationToken, out var setter))
                    {
                        this.Visit(setter);
                    }
                }
                else if (this.IsPropertySet(node))
                {
                    if (property.SetMethod.TrySingleAccessorDeclaration(this.CancellationToken, out var setter))
                    {
                        this.Visit(setter);
                    }
                }
                else if (property.GetMethod.TrySingleDeclaration(this.CancellationToken, out SyntaxNode? getter))
                {
                    this.Visit(getter);
                }
            }
        }

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            this.Visit(node?.Right);
            this.Visit(node?.Left);
        }

#pragma warning disable CA1068 // CancellationToken parameters must come last
        /// <summary>
        /// Returns a walker that have visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="create">The factory for creating a walker if not found in cache.</param>
        /// <returns>The walker that have visited <paramref name="node"/>.</returns>
        protected static T BorrowAndVisit(SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, Func<T> create)
#pragma warning restore CA1068 // CancellationToken parameters must come last
        {
            var walker = Borrow(create);

            // Not pretty below here, throwing is perhaps nicer, dunno.
            walker.SearchScope = scope == SearchScope.Member &&
                           node is TypeDeclarationSyntax ? SearchScope.Type : scope;
            if (walker.SearchScope != SearchScope.Member)
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
            walker.SearchScope = scope;
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
        /// <param name="node">The <see cref="TypeDeclarationSyntax"/>.</param>
        protected virtual void VisitTypeDeclaration(TypeDeclarationSyntax node)
        {
            if (node is null)
            {
                return;
            }

            using (var walker = TypeDeclarationWalker.Borrow(node))
            {
                foreach (var initializer in walker.Initializers)
                {
                    this.Visit(initializer);
                }

                foreach (var ctor in walker.Ctors)
                {
                    if (this.SearchScope == SearchScope.Instance &&
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

                if (this.SearchScope == SearchScope.Recursive)
                {
                    foreach (var type in walker.Types)
                    {
                        this.Visit(type);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the current context is a property set.
        /// </summary>
        /// <param name="node">The current context.</param>
        /// <returns>True if <paramref name="node"/> is found to be a property set.</returns>
        protected virtual bool IsPropertySet(IdentifierNameSyntax node)
        {
            return node.TryFirstAncestor(out AssignmentExpressionSyntax assignment) &&
                   assignment.Left.Contains(node);
        }

        /// <summary>
        /// Check if the current context is a property set.
        /// </summary>
        /// <param name="node">The current context.</param>
        /// <returns>True if <paramref name="node"/> is found to be a property set.</returns>
        protected virtual bool IsPropertyGetAndSet(IdentifierNameSyntax node)
        {
            return node.TryFirstAncestor(out PrefixUnaryExpressionSyntax _) ||
                   node.TryFirstAncestor(out PostfixUnaryExpressionSyntax _);
        }

        /// <summary>
        /// Try getting the target symbol for the node. Check if visited and that the symbol matches <see cref="SearchScope"/>.
        /// </summary>
        /// <typeparam name="TSymbol">The expected type.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="symbol">The symbol if a match.</param>
        /// <returns>True if a symbol was found.</returns>
        protected virtual bool TryGetTargetSymbol<TSymbol>(SyntaxNode node, out TSymbol symbol)
            where TSymbol : class, ISymbol
        {
            symbol = null;
            if (node is null ||
                this.SearchScope == SearchScope.Member)
            {
                return false;
            }

            if (node.TryFirstAncestor(out InvocationExpressionSyntax invocation) &&
                invocation.TryGetMethodName(out var name) &&
                name == "nameof")
            {
                return false;
            }

            if (this.SearchScope == SearchScope.Instance &&
                node.Parent is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression?.IsEither(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                return false;
            }

            if (this.visited.Add(node) &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out symbol))
            {
                if (this.SearchScope == SearchScope.Instance && symbol.IsStatic)
                {
                    return false;
                }

                if (this.SearchScope.IsEither(SearchScope.Instance, SearchScope.Type) &&
                    this.ContainingType?.IsAssignableTo(symbol.ContainingType, this.SemanticModel.Compilation) != true)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.visited.Clear();
            this.SemanticModel = null;
            this.ContainingType = null;
            this.CancellationToken = CancellationToken.None;
        }

        private class TypeDeclarationWalker : PooledWalker<TypeDeclarationWalker>
        {
#pragma warning disable SA1401 // Fields must be private
            internal readonly List<EqualsValueClauseSyntax> Initializers = new List<EqualsValueClauseSyntax>();
            internal readonly List<ConstructorDeclarationSyntax> Ctors = new List<ConstructorDeclarationSyntax>();
            internal readonly List<MemberDeclarationSyntax> Members = new List<MemberDeclarationSyntax>();
            internal readonly List<TypeDeclarationSyntax> Types = new List<TypeDeclarationSyntax>();
#pragma warning restore SA1401 // Fields must be private

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

            internal static TypeDeclarationWalker Borrow(TypeDeclarationSyntax typeDeclaration)
            {
                var walker = Borrow(() => new TypeDeclarationWalker());
                foreach (var member in typeDeclaration.Members)
                {
                    walker.Visit(member);
                }

                return walker;
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

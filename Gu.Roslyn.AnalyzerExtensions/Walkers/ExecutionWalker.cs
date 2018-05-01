namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
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

        /// <summary>
        /// Gets or sets the <see cref="CancellationToken"/>
        /// </summary>
        protected CancellationToken CancellationToken { get; set; }

        /// <inheritdoc />
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (this.Scope != Scope.Member &&
                node.Initializer == null &&
                this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var ctor) &&
                ctor.ContainingType is INamedTypeSymbol containingType &&
                Constructor.TryGetDefault(containingType.BaseType, Search.Recursive, out var defaultCtor) &&
                defaultCtor.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax declaration))
            {
                this.Visit(declaration);
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
            base.VisitObjectCreationExpression(node);
            switch (this.Scope)
            {
                case Scope.Member:
                case Scope.Instance:
                    break;
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
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
            switch (this.Scope)
            {
                case Scope.Member:
                    break;
                case Scope.Instance when TryGetTarget(out var target):
                    this.Visit(target);
                    break;
                case Scope.Recursive when TryGetTarget(out var target):
                    this.Visit(target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            bool TryGetTarget(out MethodDeclarationSyntax declaration)
            {
                declaration = null;
                if (this.Scope == Scope.Instance &&
                    node.Expression != null &&
                    !(node.Expression is InstanceExpressionSyntax))
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
                case Scope.Recursive:
                    if (this.TryGetPropertyGet(node, out var target))
                    {
                        this.Visit(target);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                case Scope.Recursive:
                    if (this.TryGetPropertyGet(node.Right, out var target))
                    {
                        this.Visit(target);
                    }

                    if (this.TryGetPropertySet(node.Left, out target))
                    {
                        this.Visit(target);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            walker.Scope = scope;
            walker.SemanticModel = semanticModel;
            walker.CancellationToken = cancellationToken;
            walker.Visit(node);
            return walker;
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.visited.Clear();
            this.Scope = Scope.Member;
            this.SemanticModel = null;
            this.CancellationToken = CancellationToken.None;
        }

        private bool TryGetPropertyGet(SyntaxNode candidate, out AccessorDeclarationSyntax result)
        {
            result = null;
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

            if (candidate.Parent is ArgumentSyntax ||
                candidate.Parent is AssignmentExpressionSyntax ||
                candidate.Parent is ExpressionStatementSyntax)
            {
                return this.SemanticModel.GetSymbolSafe(candidate, this.CancellationToken) is IPropertySymbol property &&
                       property.GetMethod is IMethodSymbol getMethod &&
                       getMethod.TrySingleDeclaration(this.CancellationToken, out result);
            }

            return false;
        }

        private bool TryGetPropertySet(SyntaxNode candidate, out AccessorDeclarationSyntax result)
        {
            result = null;
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
                return this.SemanticModel.TryGetSymbol(candidate, this.CancellationToken, out IPropertySymbol property)  &&
                       property.SetMethod is IMethodSymbol setMethod &&
                       setMethod.TrySingleDeclaration(this.CancellationToken, out result);
            }

            return false;
        }
    }
}

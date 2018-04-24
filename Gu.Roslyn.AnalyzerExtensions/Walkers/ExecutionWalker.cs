namespace Gu.Roslyn.AnalyzerExtensions
{
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
        protected Search Search { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SemanticModel"/>
        /// </summary>
        protected SemanticModel SemanticModel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CancellationToken"/>
        /// </summary>
        protected CancellationToken CancellationToken { get; set; }

        /// <inheritdoc />
        public override void Visit(SyntaxNode node)
        {
            if (node is AnonymousFunctionExpressionSyntax)
            {
                switch (node.Parent.Kind())
                {
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.Argument:
                        break;
                    default:
                        return;
                }
            }

            base.Visit(node);
        }

        /// <inheritdoc />
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
            this.VisitChained(node);
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);
            if (this.Search == Search.Recursive &&
                this.visited.Add(node) &&
                TryGetPropertyGet(node, out var getter))
            {
                this.Visit(getter);
            }

            bool TryGetPropertyGet(SyntaxNode candidate, out SyntaxNode result)
            {
                result = null;
                if (candidate.Parent is MemberAccessExpressionSyntax)
                {
                    return TryGetPropertyGet(candidate.Parent, out result);
                }

                if (candidate.Parent is ArgumentSyntax ||
                    candidate.Parent is EqualsValueClauseSyntax)
                {
                    return this.SemanticModel.GetSymbolSafe(candidate, this.CancellationToken) is IPropertySymbol property &&
                           property.GetMethod is IMethodSymbol getMethod &&
                           getMethod.TrySingleDeclaration(this.CancellationToken, out result);
                }

                return false;
            }
        }

        /// <inheritdoc />
        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            base.VisitAssignmentExpression(node);
            if (this.Search == Search.Recursive &&
                this.visited.Add(node) &&
                this.SemanticModel.GetSymbolSafe(node.Left, this.CancellationToken) is IPropertySymbol property &&
                property.TrySingleDeclaration(this.CancellationToken, out var propertyDeclaration) &&
                propertyDeclaration.TryGetSetter(out var setter))
            {
                this.Visit(setter);
            }
        }

        /// <inheritdoc />
        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            base.VisitConstructorInitializer(node);
            this.VisitChained(node);
        }

        /// <inheritdoc />
        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            base.VisitObjectCreationExpression(node);
            this.VisitChained(node);
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.visited.Clear();
            this.SemanticModel = null;
            this.CancellationToken = CancellationToken.None;
        }

        protected void VisitChained(SyntaxNode node)
        {
            if (node == null)
            {
                return;
            }

            if (this.Search == Search.Recursive &&
                this.visited.Add(node))
            {
                var method = this.SemanticModel.GetSymbolSafe(node, this.CancellationToken);
                if (method == null)
                {
                    return;
                }

                foreach (var reference in method.DeclaringSyntaxReferences)
                {
                    this.Visit(reference.GetSyntax(this.CancellationToken));
                }
            }
        }
    }
}

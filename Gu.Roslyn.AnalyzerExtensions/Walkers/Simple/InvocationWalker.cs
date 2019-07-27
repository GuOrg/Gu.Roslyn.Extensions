namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="InvocationExpressionSyntax"/> in the scope.
    /// </summary>
    public sealed class InvocationWalker : PooledWalker<InvocationWalker>
    {
        private readonly List<InvocationExpressionSyntax> invocations = new List<InvocationExpressionSyntax>();

        private InvocationWalker()
        {
        }

        /// <summary>
        /// A collection with the <see cref="InvocationExpressionSyntax"/> found when walking.
        /// </summary>
        public IReadOnlyList<InvocationExpressionSyntax> Invocations => this.invocations;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static InvocationWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new InvocationWalker());

        /// <inheritdoc />
        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            this.invocations.Add(node);
            base.VisitInvocationExpression(node);
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.invocations.Clear();
        }
    }
}

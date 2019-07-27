namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="ThrowStatementSyntax"/> and <see cref="ThrowExpressionSyntax"/> in the scope.
    /// </summary>
    public sealed class ThrowWalker : PooledWalker<ThrowWalker>
    {
        private readonly List<SyntaxToken> throwStatementsAndExpressions = new List<SyntaxToken>();

        private ThrowWalker()
        {
        }

        /// <summary>
        /// Gets a collection with the <see cref="ThrowStatementSyntax"/> and <see cref="ThrowExpressionSyntax"/> found when walking.
        /// </summary>
        public IReadOnlyList<SyntaxToken> ThrowStatementsAndExpressions => this.throwStatementsAndExpressions;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static ThrowWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new ThrowWalker());

        /// <summary>
        /// Check if there is a <see cref="ThrowStatementSyntax"/> or <see cref="ThrowExpressionSyntax"/> in <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>True if there is at least one <see cref="ThrowStatementSyntax"/> or <see cref="ThrowExpressionSyntax"/> in <paramref name="node"/>.</returns>
        public static bool Throws(SyntaxNode node)
        {
            if (node == null)
            {
                return false;
            }

            using (var walker = Borrow(node))
            {
                return walker.throwStatementsAndExpressions.Count > 0;
            }
        }

        /// <inheritdoc/>
        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            this.throwStatementsAndExpressions.Add(node.ThrowKeyword);
            base.VisitThrowStatement(node);
        }

        /// <inheritdoc/>
        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            this.throwStatementsAndExpressions.Add(node.ThrowKeyword);
            base.VisitThrowExpression(node);
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.throwStatementsAndExpressions.Clear();
        }
    }
}

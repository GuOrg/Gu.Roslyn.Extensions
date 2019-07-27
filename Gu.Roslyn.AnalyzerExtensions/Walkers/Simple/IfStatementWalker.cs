namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="IfStatementSyntax"/> in the scope.
    /// </summary>
    public sealed class IfStatementWalker : PooledWalker<IfStatementWalker>
    {
        private readonly List<IfStatementSyntax> ifStatements = new List<IfStatementSyntax>();

        private IfStatementWalker()
        {
        }

        /// <summary>
        /// Gets a list with all <see cref="IfStatementSyntax"/> in the scope.
        /// </summary>
        public IReadOnlyList<IfStatementSyntax> IfStatements => this.ifStatements;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static IfStatementWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new IfStatementWalker());

        /// <inheritdoc />
        public override void VisitIfStatement(IfStatementSyntax node)
        {
            this.ifStatements.Add(node);
            base.VisitIfStatement(node);
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.ifStatements.Clear();
        }
    }
}

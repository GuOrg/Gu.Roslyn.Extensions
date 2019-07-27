namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="UsingStatementSyntax"/> in the scope.
    /// </summary>
    public sealed class UsingStatementWalker : PooledWalker<UsingStatementWalker>
    {
        private readonly List<UsingStatementSyntax> usingStatements = new List<UsingStatementSyntax>();

        private UsingStatementWalker()
        {
        }

        /// <summary>
        /// Gets a collection with the <see cref="UsingStatementSyntax"/> found when walking.
        /// </summary>
        public IReadOnlyList<UsingStatementSyntax> UsingStatements => this.usingStatements;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static UsingStatementWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new UsingStatementWalker());

        /// <inheritdoc/>
        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            this.usingStatements.Add(node);
            base.VisitUsingStatement(node);
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.usingStatements.Clear();
        }
    }
}

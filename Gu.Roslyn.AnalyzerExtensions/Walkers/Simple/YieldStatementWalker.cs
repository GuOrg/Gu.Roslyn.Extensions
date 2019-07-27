namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="UsingStatementSyntax"/> in the scope.
    /// </summary>
    public sealed class YieldStatementWalker : PooledWalker<YieldStatementWalker>
    {
        private readonly List<YieldStatementSyntax> yieldStatements = new List<YieldStatementSyntax>();

        private YieldStatementWalker()
        {
        }

        /// <summary>
        /// Gets a list with all <see cref="YieldStatementSyntax"/> in the scope.
        /// </summary>
        public IReadOnlyList<YieldStatementSyntax> YieldStatements => this.yieldStatements;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static YieldStatementWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new YieldStatementWalker());

        /// <summary>
        /// Check if <paramref name="methodDeclaration"/> contains any <see cref="YieldStatementSyntax"/>.
        /// </summary>
        /// <param name="methodDeclaration">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <returns>True if <see cref="YieldStatementSyntax"/> was found.</returns>
        public static bool Any(MethodDeclarationSyntax methodDeclaration)
        {
            using (var walker = Borrow(methodDeclaration))
            {
                return walker.yieldStatements.Count > 0;
            }
        }

        /// <inheritdoc/>
        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            this.yieldStatements.Add(node);
            base.VisitYieldStatement(node);
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.yieldStatements.Clear();
        }
    }
}
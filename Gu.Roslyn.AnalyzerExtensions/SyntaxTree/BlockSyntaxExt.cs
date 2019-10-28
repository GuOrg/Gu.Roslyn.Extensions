namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="BlockSyntax"/>.
    /// </summary>
    public static class BlockSyntaxExt
    {
        /// <summary>
        /// Check if the block contains a GotoStatement or LabeledStatement recursively.
        /// </summary>
        /// <param name="block">The <see cref="BlockSyntax"/>.</param>
        /// <returns>True if a goto or label is found.</returns>
        public static bool ContainsGoto(this BlockSyntax block)
        {
            if (block is null)
            {
                return false;
            }

            foreach (var candidate in block.Statements)
            {
                if (candidate.IsKind(SyntaxKind.GotoStatement) ||
                    candidate.IsKind(SyntaxKind.LabeledStatement))
                {
                    return true;
                }

                if (candidate is IfStatementSyntax ifStatement &&
                    (ContainsGoto(ifStatement.Statement as BlockSyntax) ||
                     ContainsGoto(ifStatement.Else?.Statement as BlockSyntax)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

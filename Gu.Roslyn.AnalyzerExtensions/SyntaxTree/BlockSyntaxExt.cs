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

                switch (candidate)
                {
                    case IfStatementSyntax { Statement: BlockSyntax whenTrue } when ContainsGoto(whenTrue):
                        return true;
                    case IfStatementSyntax { Else: { Statement: BlockSyntax elseBlock } } when ContainsGoto(elseBlock):
                        return true;
                }
            }

            return false;
        }
    }
}

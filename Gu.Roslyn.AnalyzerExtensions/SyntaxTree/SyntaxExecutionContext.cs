namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// For checking if execution is in anonymous functions.
    /// </summary>
    internal static class SyntaxExecutionContext
    {
        /// <summary>
        /// Check if <paramref name="node"/> is in lambda.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="other">The other <see cref="SyntaxNode"/>.</param>
        /// <param name="executedBefore">The execution order.</param>
        /// <returns>True if any or both nodes are in lambda.</returns>
        internal static bool IsInLambda(SyntaxNode node, SyntaxNode other, out ExecutedBefore executedBefore)
        {
            if (node.TryFirstAncestor(out AnonymousFunctionExpressionSyntax? nodeLambda))
            {
                if (other.TryFirstAncestor(out AnonymousFunctionExpressionSyntax? otherLambda))
                {
                    if (ReferenceEquals(nodeLambda, otherLambda))
                    {
                        // in the same lambda we handle it like normal execution.
                        executedBefore = ExecutedBefore.Unknown;
                        return false;
                    }

                    executedBefore = ExecutedBefore.Maybe;
                    return true;
                }

                executedBefore = node.SpanStart > other.SpanStart ? ExecutedBefore.No : ExecutedBefore.Maybe;
                return true;
            }

            if (other.TryFirstAncestor<AnonymousFunctionExpressionSyntax>(out _))
            {
                executedBefore = node.SpanStart < other.SpanStart ? ExecutedBefore.Yes : ExecutedBefore.Maybe;
                return true;
            }

            executedBefore = ExecutedBefore.Unknown;
            return false;
        }
    }
}

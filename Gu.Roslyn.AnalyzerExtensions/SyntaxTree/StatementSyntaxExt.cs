namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="StatementSyntax"/>.
    /// </summary>
    public static class StatementSyntaxExt
    {
        /// <summary>
        /// Check if <paramref name="statement"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="statement">The <see cref="StatementSyntax"/>.</param>
        /// <param name="other">The other <see cref="StatementSyntax"/>.</param>
        /// <returns>Null if the execution order could not be figured out.</returns>
        public static bool? IsExecutedBefore(this StatementSyntax statement, StatementSyntax other)
        {
            if (statement == null ||
                other == null ||
                ReferenceEquals(statement, other))
            {
                return false;
            }

            if ((statement.Parent as BlockSyntax)?.ContainsGoto() == true)
            {
                return null;
            }

            if (!statement.SharesAncestor<MemberDeclarationSyntax>(other, out _))
            {
                return null;
            }

            if (ReferenceEquals(statement.Parent, other.Parent))
            {
                return statement.SpanStart < other.SpanStart;
            }

            if (statement.IsInParentBlock(other))
            {
                if (statement.SpanStart < other.SpanStart)
                {
                    return true;
                }

                if (other.TryFirstAncestor(out AnonymousFunctionExpressionSyntax _))
                {
                    return null;
                }

                return false;
            }

            if (other.IsInParentBlock(statement))
            {
                if (statement.SpanStart > other.SpanStart)
                {
                    return false;
                }

                if (statement.TryFirstAncestor(out AnonymousFunctionExpressionSyntax _))
                {
                    return null;
                }

                if (statement.Parent is BlockSyntax block &&
                    (block.Statements.TryFirstOfType(out ReturnStatementSyntax _) ||
                     block.Statements.TryFirstOfType(out ThrowStatementSyntax _)))
                {
                    return false;
                }

                return statement.SpanStart < other.SpanStart;
            }

            if (statement.TryFindSharedAncestorRecursive(other, out IfStatementSyntax ifStatement) &&
                ((ifStatement.Statement?.Contains(statement) == true && ifStatement.Else?.Statement?.Contains(other) == true) ||
                 (ifStatement.Statement?.Contains(other) == true && ifStatement.Else?.Statement?.Contains(statement) == true)))
            {
                return false;
            }

            return null;
        }

        /// <summary>
        /// Check if <paramref name="statement"/> or statement.Parent contains the block <paramref name="other"/> is in.
        /// </summary>
        /// <param name="statement">The <see cref="StatementSyntax"/>.</param>
        /// <param name="other">The other <see cref="StatementSyntax"/>.</param>
        /// <returns>True if <paramref name="statement"/> or statement.Parent contains the block <paramref name="other"/> is in.</returns>
        public static bool IsInParentBlock(this StatementSyntax statement, StatementSyntax other)
        {
            if (statement == null || other == null)
            {
                return false;
            }

            if (ReferenceEquals(statement, other) ||
                ReferenceEquals(statement.Parent, other.Parent))
            {
                return false;
            }

            return statement.Parent is BlockSyntax block &&
                   block.Contains(other);
        }
    }
}

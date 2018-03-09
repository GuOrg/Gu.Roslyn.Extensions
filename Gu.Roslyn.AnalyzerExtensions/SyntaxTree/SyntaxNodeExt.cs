namespace Gu.Roslyn.AnalyzerExtensions.SyntaxTree
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class SyntaxNodeExt
    {
        public static T FirstAncestor<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            if (node == null)
            {
                return null;
            }

            if (node is T)
            {
                return node.Parent?.FirstAncestorOrSelf<T>();
            }

            return node.FirstAncestorOrSelf<T>();
        }

        public static bool? IsBeforeInScope(this SyntaxNode node, SyntaxNode other)
        {
            var statement = node?.FirstAncestorOrSelf<StatementSyntax>();
            var otherStatement = other?.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null ||
                otherStatement == null)
            {
                return null;
            }

            var block = statement.Parent as BlockSyntax;
            var otherBlock = otherStatement.Parent as BlockSyntax;
            if (block == null && otherBlock == null)
            {
                return false;
            }

            if (ReferenceEquals(block, otherBlock) ||
                otherBlock?.Contains(node) == true ||
                block?.Contains(other) == true)
            {
                var firstAnon = node.FirstAncestor<AnonymousFunctionExpressionSyntax>();
                var otherAnon = other.FirstAncestor<AnonymousFunctionExpressionSyntax>();
                if (!ReferenceEquals(firstAnon, otherAnon))
                {
                    return true;
                }

                return statement.SpanStart < otherStatement.SpanStart;
            }

            return false;
        }

        internal static bool SharesAncestor<T>(this SyntaxNode first, SyntaxNode other)
            where T : SyntaxNode
        {
            var firstAncestor = first.FirstAncestor<T>();
            var otherAncestor = other.FirstAncestor<T>();
            if (firstAncestor == null ||
                otherAncestor == null)
            {
                return false;
            }

            return firstAncestor == otherAncestor;
        }
    }
}

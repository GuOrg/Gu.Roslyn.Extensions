namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public static class SyntaxNodeExt
    {
        public static T FirstAncestor<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            if (node == null)
            {
                return null;
            }

            var ancestor = node.FirstAncestorOrSelf<T>();
            return ReferenceEquals(ancestor, node)
                ? node.Parent?.FirstAncestorOrSelf<T>()
                : ancestor;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helpers for working with <see cref="SeparatedSyntaxList{TNode}"/>.
    /// </summary>
    public static class SeparatedSyntaxListExt
    {
        public static T First<T>(this SeparatedSyntaxList<T> list, SyntaxKind kind)
            where T : SyntaxNode
        {
            foreach (T node in list)
            {
                if (node.IsKind(kind))
                {
                    return node;
                }
            }

            throw new InvalidOperationException($"Did not find a node with kind {kind}");
        }

        public static T? FirstOrDefault<T>(this SeparatedSyntaxList<T> list, SyntaxKind kind)
            where T : SyntaxNode
        {
            foreach (T node in list)
            {
                if (node.IsKind(kind))
                {
                    return node;
                }
            }

            return null;
        }
    }
}

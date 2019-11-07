namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxList{TNode}"/>.
    /// </summary>
    public static class SyntaxListExt
    {
        /// <summary>
        /// Get the first element with kind <paramref name="kind"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The <see cref="SyntaxList{T}"/>.</param>
        /// <param name="kind">The <see cref="SyntaxKind"/>.</param>
        /// <returns>A <typeparamref name="T"/> if a match was found.</returns>
        public static T First<T>(this SyntaxList<T> list, SyntaxKind kind)
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

        /// <summary>
        /// Get the first element with kind <paramref name="kind"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The <see cref="SyntaxList{T}"/>.</param>
        /// <param name="kind">The <see cref="SyntaxKind"/>.</param>
        /// <returns>A <typeparamref name="T"/> if a match was found.</returns>
        public static T? FirstOrDefault<T>(this SyntaxList<T> list, SyntaxKind kind)
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

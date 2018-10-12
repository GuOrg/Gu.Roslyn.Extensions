namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Extension methods for finding matching node for diagnostics.
    /// </summary>
    public static class SyntaxNodeExt
    {
        /// <summary>
        /// syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as T.
        /// </summary>
        /// <typeparam name="T">The type of node to find.</typeparam>
        /// <param name="syntaxRoot">The syntax root of the document containing the diagnostic.</param>
        /// <param name="diagnostic">The <see cref="Diagnostic"/>.</param>
        /// <param name="node">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindNode<T>(this SyntaxNode syntaxRoot, Diagnostic diagnostic, out T node)
            where T : SyntaxNode
        {
            var candidate = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            while (candidate.Span == diagnostic.Location.SourceSpan)
            {
                if (candidate is T match)
                {
                    node = match;
                    return true;
                }

                candidate = candidate.Parent;
            }

            node = null;
            return false;
        }

        /// <summary>
        /// syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true)
        ///           .FirstAncestorOrSelf{T}().
        /// </summary>
        /// <typeparam name="T">The type of node to find.</typeparam>
        /// <param name="syntaxRoot">The syntax root of the document containing the diagnostic.</param>
        /// <param name="diagnostic">The <see cref="Diagnostic"/>.</param>
        /// <param name="node">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindNodeOrAncestor<T>(this SyntaxNode syntaxRoot, Diagnostic diagnostic, out T node)
            where T : SyntaxNode
        {
            node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true)
                             .FirstAncestorOrSelf<T>();
            return node != null;
        }
    }
}

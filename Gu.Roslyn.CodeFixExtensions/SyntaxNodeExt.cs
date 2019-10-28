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
            if (diagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(diagnostic));
            }

            return TryFindNode(syntaxRoot, diagnostic.Location, out node);
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
            if (diagnostic is null)
            {
                throw new System.ArgumentNullException(nameof(diagnostic));
            }

            return TryFindNodeOrAncestor(syntaxRoot, diagnostic.Location, out node);
        }

        /// <summary>
        /// syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as T.
        /// </summary>
        /// <typeparam name="T">The type of node to find.</typeparam>
        /// <param name="syntaxRoot">The syntax root of the document containing the diagnostic.</param>
        /// <param name="location">The <see cref="Location"/>.</param>
        /// <param name="node">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindNode<T>(this SyntaxNode syntaxRoot, Location location, out T node)
            where T : SyntaxNode
        {
            if (syntaxRoot == null)
            {
                throw new System.ArgumentNullException(nameof(syntaxRoot));
            }

            if (location == null)
            {
                throw new System.ArgumentNullException(nameof(location));
            }

            var candidate = syntaxRoot.FindNode(location.SourceSpan, getInnermostNodeForTie: true);
            while (candidate.Span == location.SourceSpan)
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
        /// <param name="location">The <see cref="Location"/>.</param>
        /// <param name="node">The match.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindNodeOrAncestor<T>(this SyntaxNode syntaxRoot, Location location, out T node)
            where T : SyntaxNode
        {
            if (syntaxRoot == null)
            {
                throw new System.ArgumentNullException(nameof(syntaxRoot));
            }

            if (location == null)
            {
                throw new System.ArgumentNullException(nameof(location));
            }

            node = syntaxRoot.FindNode(location.SourceSpan, getInnermostNodeForTie: true)
                             .FirstAncestorOrSelf<T>();
            return node != null;
        }
    }
}

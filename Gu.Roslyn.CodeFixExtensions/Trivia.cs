namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helper for processing <see cref="SyntaxTrivia"/>.
    /// </summary>
    public static class Trivia
    {
        /// <summary>
        /// Copy leading trivia from <paramref name="target"/> to <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="target">The target to copy trivia to.</param>
        /// <param name="source">The source to copy trivia from.</param>
        /// <returns><paramref name="target"/> with trivia from <paramref name="source"/>.</returns>
        public static T WithLeadingTriviaFrom<T>(this T target, SyntaxNode source)
            where T : SyntaxNode
        {
            return source.HasLeadingTrivia
                ? target.WithLeadingTrivia(source.GetLeadingTrivia())
                : target;
        }

        /// <summary>
        /// Copy trailing trivia from <paramref name="target"/> to <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="target">The target to copy trivia to.</param>
        /// <param name="source">The source to copy trivia from.</param>
        /// <returns><paramref name="target"/> with trivia from <paramref name="source"/>.</returns>
        public static T WithTrailingTriviaFrom<T>(this T target, SyntaxNode source)
            where T : SyntaxNode
        {
            return source.HasTrailingTrivia
                ? target.WithTrailingTrivia(source.GetTrailingTrivia())
                : target;
        }

        /// <summary>
        /// Add leading elastic line feed to <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns><paramref name="node"/> with leading elastic line feed.</returns>
        public static T WithLeadingElasticLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.ElasticLineFeed));
            }

            return node.WithLeadingTrivia(SyntaxFactory.ElasticLineFeed);
        }

        /// <summary>
        /// Add leading line feed to <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns><paramref name="node"/> with leading line feed.</returns>
        public static T WithLeadingLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.LineFeed));
            }

            return node.WithLeadingTrivia(SyntaxFactory.LineFeed);
        }

        /// <summary>
        /// Add trailing elastic line feed to <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns><paramref name="node"/> with trailing elastic line feed.</returns>
        public static T WithTrailingElasticLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(
                    node.GetTrailingTrivia()
                        .Add(SyntaxFactory.ElasticLineFeed));
            }

            return node.WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
        }

        /// <summary>
        /// Add trailing line feed to <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns><paramref name="node"/> with trailing line feed.</returns>
        public static T WithTrailingLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(
                    node.GetTrailingTrivia()
                        .Add(SyntaxFactory.LineFeed));
            }

            return node.WithTrailingTrivia(SyntaxFactory.LineFeed);
        }
    }
}

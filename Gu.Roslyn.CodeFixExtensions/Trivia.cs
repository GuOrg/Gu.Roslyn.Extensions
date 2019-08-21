namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for processing <see cref="SyntaxTrivia"/>.
    /// </summary>
    public static class Trivia
    {
        /// <summary>
        /// Get the leading <see cref="SyntaxKind.EndOfLineTrivia"/> if exists.
        /// </summary>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="trivia">The <see cref="SyntaxTrivia"/>.</param>
        /// <returns>True if leading <see cref="SyntaxKind.EndOfLineTrivia"/> exists.</returns>
        public static bool TryGetLeadingTrivia(this MemberDeclarationSyntax member, out SyntaxTriviaList trivia)
        {
            if (member == null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            if (member.HasLeadingTrivia)
            {
                trivia = member.GetLeadingTrivia();
                return true;
            }

            trivia = default;
            return false;
        }

        /// <summary>
        /// Get the leading <see cref="SyntaxKind.EndOfLineTrivia"/> if exists.
        /// </summary>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="trivia">The <see cref="SyntaxTrivia"/>.</param>
        /// <returns>True if leading <see cref="SyntaxKind.EndOfLineTrivia"/> exists.</returns>
        public static bool TryGetTrailingTrivia(this MemberDeclarationSyntax member, out SyntaxTriviaList trivia)
        {
            if (member == null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            if (member.HasTrailingTrivia)
            {
                trivia = member.GetTrailingTrivia();
                return true;
            }

            trivia = default;
            return false;
        }

        /// <summary>
        /// Get the leading <see cref="SyntaxKind.EndOfLineTrivia"/> if exists.
        /// </summary>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="trivia">The <see cref="SyntaxTrivia"/>.</param>
        /// <returns>True if leading <see cref="SyntaxKind.EndOfLineTrivia"/> exists.</returns>
        public static bool TryGetLeadingNewLine(this MemberDeclarationSyntax member, out SyntaxTrivia trivia)
        {
            if (member == null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            trivia = default;
            return member.HasLeadingTrivia &&
                   member.GetLeadingTrivia() is SyntaxTriviaList triviaList &&
                   triviaList.TryFirst(out trivia) &&
                   trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        /// <summary>
        /// Get the trailing <see cref="SyntaxKind.EndOfLineTrivia"/> if exists.
        /// </summary>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="trivia">The <see cref="SyntaxTrivia"/>.</param>
        /// <returns>True if trailing <see cref="SyntaxKind.EndOfLineTrivia"/> exists.</returns>
        public static bool TryGetTrailingNewLine(this MemberDeclarationSyntax member, out SyntaxTrivia trivia)
        {
            if (member == null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            trivia = default;
            return member.HasTrailingTrivia &&
                   member.GetTrailingTrivia() is SyntaxTriviaList triviaList &&
                   triviaList.TryLast(out trivia) &&
                   trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> before existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T PrependLeadingTrivia<T>(this T node, params SyntaxTrivia[] trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(trivia.Concat(node.GetLeadingTrivia()));
            }

            return node.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> before existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T PrependLeadingTrivia<T>(this T node, IEnumerable<SyntaxTrivia> trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(trivia.Concat(node.GetLeadingTrivia()));
            }

            return node.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> after existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T AppendLeadingTrivia<T>(this T node, params SyntaxTrivia[] trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(node.GetLeadingTrivia().Concat(trivia));
            }

            return node.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> after existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T AppendLeadingTrivia<T>(this T node, IEnumerable<SyntaxTrivia> trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(node.GetLeadingTrivia().Concat(trivia));
            }

            return node.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> before existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T PrependTrailingTrivia<T>(this T node, params SyntaxTrivia[] trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(trivia.Concat(node.GetTrailingTrivia()));
            }

            return node.WithTrailingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> before existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T PrependTrailingTrivia<T>(this T node, IEnumerable<SyntaxTrivia> trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(trivia.Concat(node.GetTrailingTrivia()));
            }

            return node.WithTrailingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> after existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T AppendTrailingTrivia<T>(this T node, params SyntaxTrivia[] trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(node.GetTrailingTrivia().Concat(trivia));
            }

            return node.WithTrailingTrivia(trivia);
        }

        /// <summary>
        /// Add <paramref name="trivia"/> after existing trivia.
        /// </summary>
        /// <typeparam name="T">The <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <typeparamref name="T"/>.</param>
        /// <param name="trivia">The array of <see cref="SyntaxTrivia"/>.</param>
        /// <returns>The node with updated trivia.</returns>
        public static T AppendTrailingTrivia<T>(this T node, IEnumerable<SyntaxTrivia> trivia)
            where T : SyntaxNode
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(node.GetTrailingTrivia().Concat(trivia));
            }

            return node.WithTrailingTrivia(trivia);
        }

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
            if (source == null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

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
            if (source == null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

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
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.ElasticLineFeed));
            }

            return node.WithLeadingTrivia(SyntaxFactory.ElasticLineFeed);
        }

        /// <summary>
        /// Add leading elastic line feed to <paramref name="node"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="SyntaxNode"/>.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns><paramref name="node"/> with leading elastic line feed.</returns>
        public static T WithLeadingElasticSpace<T>(this T node)
            where T : SyntaxNode
        {
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.ElasticSpace));
            }

            return node.WithLeadingTrivia(SyntaxFactory.ElasticSpace);
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
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

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
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

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
            if (node == null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

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

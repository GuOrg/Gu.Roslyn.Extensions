namespace Gu.Roslyn.CodeFixExtensions
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for modifying <see cref="MemberDeclarationSyntax"/>.
    /// </summary>
    public static partial class MemberDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Add <paramref name="text"/> as leading trivia to <paramref name="member"/>.
        /// </summary>
        /// <typeparam name="TMember">The type of the member to add docs to.</typeparam>
        /// <param name="member">The <typeparamref name="TMember"/>.</param>
        /// <param name="text">The text to parse into <see cref="SyntaxTriviaList"/>.</param>
        /// <param name="adjustLeadingWhitespace">If true leading whitespaces is adjusted to match <paramref name="member"/>.</param>
        /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
        public static TMember WithDocumentationText<TMember>(this TMember member, string text, bool adjustLeadingWhitespace = true)
            where TMember : MemberDeclarationSyntax
        {
            if (member is null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            var leadingTrivia = Parse.LeadingTrivia(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null);
            if (member.HasLeadingTrivia &&
                leadingTrivia.TrySingle(x => x.HasStructure && x.GetStructure() is DocumentationCommentTriviaSyntax, out var withStructure) &&
                withStructure.GetStructure() is DocumentationCommentTriviaSyntax docs)
            {
                return member.WithDocs(docs);
            }

            return member.WithLeadingTrivia(leadingTrivia);
        }

        /// <summary>
        /// Add docs as leading trivia to <paramref name="member"/>.
        /// </summary>
        /// <typeparam name="TMember">The type of the member to add docs to.</typeparam>
        /// <param name="member">The <typeparamref name="TMember"/>.</param>
        /// <param name="docs">The <see cref="DocumentationCommentTriviaSyntax"/>.</param>
        /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
        public static TMember WithDocs<TMember>(this TMember member, DocumentationCommentTriviaSyntax docs)
            where TMember : MemberDeclarationSyntax
        {
            if (member is null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            if (member.HasLeadingTrivia &&
                member.GetLeadingTrivia() is var triviaList)
            {
                if (triviaList.TrySingle(x => x.HasStructure && x.GetStructure() is DocumentationCommentTriviaSyntax, out var trivia))
                {
                    return member.WithLeadingTrivia(triviaList.Replace(trivia, SyntaxFactory.Trivia(docs)));
                }

                if (triviaList.TryFirst(x => x.IsKind(SyntaxKind.WhitespaceTrivia), out var first))
                {
                    return member.WithLeadingTrivia(triviaList.AddRange(new[] { SyntaxFactory.Trivia(docs), first }));
                }

                return member.WithLeadingTrivia(triviaList.Add(SyntaxFactory.Trivia(docs)));
            }

            return member.WithLeadingTrivia(SyntaxFactory.Trivia(docs));
        }

        /// <summary>
        /// Adjust leading whitespace for <paramref name="member"/> to match StyleCop.
        /// </summary>
        /// <typeparam name="TMember">The type of <paramref name="member"/>.</typeparam>
        /// <param name="member">The <typeparamref name="TMember"/>.</param>
        /// <param name="previous">The previous member or null if <paramref name="member"/> is the first in the type.</param>
        /// <returns><paramref name="member"/> with adjusted leading newline.</returns>
        public static TMember AdjustLeadingNewLine<TMember>(this TMember member, TMember? previous)
            where TMember : MemberDeclarationSyntax
        {
            if (previous is null)
            {
                return member.WithoutLeadingLineFeed();
            }

            if (member is FieldDeclarationSyntax field &&
                previous is FieldDeclarationSyntax previousField &&
               Equal(field.Modifiers, previousField.Modifiers))
            {
                return member.WithoutLeadingLineFeed();
            }

            return member.TryGetLeadingNewLine(out _)
                ? member
                : member.WithLeadingLineFeed();

            static bool Equal(SyntaxTokenList x, SyntaxTokenList y)
            {
                if (x.Count == y.Count)
                {
                    for (var i = 0; i < x.Count; i++)
                    {
                        if (x[i].Kind() != y[i].Kind())
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}

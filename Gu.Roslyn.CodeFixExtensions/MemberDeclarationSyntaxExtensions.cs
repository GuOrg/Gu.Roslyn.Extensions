namespace Gu.Roslyn.CodeFixExtensions
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for modifying <see cref="MemberDeclarationSyntax"/>
    /// </summary>
    public static class MemberDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Add <paramref name="text"/> as leading trivia to <paramref name="member"/>
        /// </summary>
        /// <typeparam name="TNode">The type of the member to add docs to.</typeparam>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/></param>
        /// <param name="text">The text to parse into <see cref="SyntaxTriviaList"/></param>
        /// <param name="adjustLeadingWhitespace">If true leading whitespaces is adjusted to match <paramref name="member"/></param>
        /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
        public static TNode WithDocumentationText<TNode>(this TNode member, string text, bool adjustLeadingWhitespace = true)
            where TNode : MemberDeclarationSyntax
        {
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
        /// Add docs as leading trivia to <paramref name="member"/>
        /// </summary>
        /// <typeparam name="TNode">The type of the member to add docs to.</typeparam>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/></param>
        /// <param name="docs">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
        public static TNode WithDocs<TNode>(this TNode member, DocumentationCommentTriviaSyntax docs)
            where TNode : MemberDeclarationSyntax
        {
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
        /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
        /// </summary>
        /// <param name="member">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="text">
        /// The attribute text including start and end [].
        /// </param>
        /// <param name="adjustLeadingWhitespace">If true leading whitespaces is adjusted to match <paramref name="member"/></param>
        /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
        public static MethodDeclarationSyntax WithAttributeListText(this MethodDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
        {
            return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
        }

        /// <summary>
        /// Add the attribute list to the <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        /// <param name="member">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
        /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
        public static MethodDeclarationSyntax WithAttributeList(this MethodDeclarationSyntax member, AttributeListSyntax attributeList)
        {
            return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
        }
    }
}

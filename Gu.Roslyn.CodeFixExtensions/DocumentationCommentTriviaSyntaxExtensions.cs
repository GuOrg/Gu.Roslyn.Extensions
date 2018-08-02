namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for <see cref="DocumentationCommentTriviaSyntax"/>
    /// </summary>
    public static class DocumentationCommentTriviaSyntaxExtensions
    {
        /// <summary>
        /// Add a summary element to <paramref name="comment"/>
        /// Replace if a summary element exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="text"> The text to add inside the summary element.</param>
        /// <returns><paramref name="comment"/> with summary.</returns>
        public static DocumentationCommentTriviaSyntax WithSummary(this DocumentationCommentTriviaSyntax comment, string text)
        {
            return comment.WithSummary(Parse.XmlElementSyntax($"<summary> {text} </summary>"));
        }

        /// <summary>
        /// Add <paramref name="summary"/> element to <paramref name="comment"/>
        /// Replace if a summary element exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="summary"> The <see cref="XmlElementSyntax"/>.</param>
        /// <returns><paramref name="comment"/> with <paramref name="summary"/>.</returns>
        public static DocumentationCommentTriviaSyntax WithSummary(this DocumentationCommentTriviaSyntax comment, XmlElementSyntax summary)
        {
            if (comment.TryGetSummary(out var old))
            {
                return comment.ReplaceNode(old, summary);
            }

            return comment.WithContent(comment.Content.InsertRange(1, new XmlNodeSyntax[] { summary, comment.NewLine() }));
        }

        private static XmlTextSyntax NewLine(this DocumentationCommentTriviaSyntax comment)
        {
            var leadingWhiteSpace = comment.ParentTrivia.Token.LeadingTrivia.TryFirst(x => x.IsKind(SyntaxKind.WhitespaceTrivia), out var whitespaceTrivia)
                    ? whitespaceTrivia.ToString()
                    : "        ";
            return SyntaxFactory.XmlText(
                                    SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.XmlTextLiteralNewLineToken, Environment.NewLine, Environment.NewLine, SyntaxTriviaList.Empty),
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, $"{leadingWhiteSpace}///")),
                                        kind: SyntaxKind.XmlTextLiteralToken,
                                        text: " ",
                                        valueText: " ",
                                        trailing: SyntaxTriviaList.Empty))
                                .WithLeadingTrivia(SyntaxTriviaList.Empty);
        }
    }
}

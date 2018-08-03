namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Linq;
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
        public static DocumentationCommentTriviaSyntax WithSummaryText(this DocumentationCommentTriviaSyntax comment, string text)
        {
            return comment.WithSummary(Parse.XmlElementSyntax(CreateElementXml(text, "summary"), comment.LeadingWhitespace()));
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

        /// <summary>
        /// Add a summary element to <paramref name="comment"/>
        /// Replace if a summary element exists.
        /// If the comment is attached to a method the param element is inserted at the correct position.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="text"> The text to add inside the summary element.</param>
        /// <returns><paramref name="comment"/> with summary.</returns>
        public static DocumentationCommentTriviaSyntax WithParamText(this DocumentationCommentTriviaSyntax comment, string parameterName, string text)
        {
            return comment.WithParam(Parse.XmlElementSyntax(CreateElementXml(text, "param", parameterName), comment.LeadingWhitespace()));
        }

        /// <summary>
        /// Add <paramref name="param"/> element to <paramref name="comment"/>
        /// Replace if a summary element exists.
        /// If the comment is attached to a method the param element is inserted at the correct position.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="param"> The <see cref="XmlElementSyntax"/>.</param>
        /// <returns><paramref name="comment"/> with <paramref name="param"/>.</returns>
        public static DocumentationCommentTriviaSyntax WithParam(this DocumentationCommentTriviaSyntax comment, XmlElementSyntax param)
        {
            if (param.TryGetNameAttribute(out var attribute) &&
                attribute.Identifier is IdentifierNameSyntax identifierName)
            {
                if (comment.TryGetParam(identifierName.Identifier.ValueText, out var old))
                {
                    return comment.ReplaceNode(old, param);
                }

                return comment.WithContent(comment.Content.InsertRange(FindPosition(), new XmlNodeSyntax[] { param, comment.NewLine() }));
            }

            throw new ArgumentException("Element does not have a name attribute.", nameof(param));


            int FindPosition()
            {
                if (comment.TryFirstAncestor(out MethodDeclarationSyntax method) &&
                    method.TryFindParameter(identifierName.Identifier.ValueText, out var parameter))
                {
                    var index = method.ParameterList.Parameters.IndexOf(parameter);
                    return comment.Content.Count(IsBefore) - 1;

                    bool IsBefore(XmlNodeSyntax node)
                    {
                        if (node is XmlElementSyntax e)
                        {
                            if (e.StartTag is XmlElementStartTagSyntax startTag &&
                                startTag.Name is XmlNameSyntax nameSyntax)
                            {
                                if (string.Equals("summary", nameSyntax.LocalName.ValueText, StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals("typeparam", nameSyntax.LocalName.ValueText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }

                                if (string.Equals("returns", nameSyntax.LocalName.ValueText, StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals("exception", nameSyntax.LocalName.ValueText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return false;
                                }

                                if (nameSyntax.LocalName.ValueText == "param" &&
                                    startTag.Attributes.TrySingleOfType(out XmlNameAttributeSyntax nameAttribute) &&
                                    method.TryFindParameter(nameAttribute.Identifier.Identifier.ValueText, out var other))
                                {
                                    return method.ParameterList.Parameters.IndexOf(other) < index;
                                }

                            }

                            return false;
                        }

                        return true;
                    }
                }

                return comment.Content.Count - 1;
            }
        }

        private static XmlTextSyntax NewLine(this DocumentationCommentTriviaSyntax comment)
        {
            return SyntaxFactory.XmlText(
                                    SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.XmlTextLiteralNewLineToken, Environment.NewLine, Environment.NewLine, SyntaxTriviaList.Empty),
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, $"{comment.LeadingWhitespace()}///")),
                                        kind: SyntaxKind.XmlTextLiteralToken,
                                        text: " ",
                                        valueText: " ",
                                        trailing: SyntaxTriviaList.Empty))
                                .WithLeadingTrivia(SyntaxTriviaList.Empty);
        }

        private static string CreateElementXml(string content, string localName)
        {
            if (content.IndexOf('\n') < 0)
            {
                return $"<{localName}>{content}</{localName}>";
            }

            return StringBuilderPool.Borrow()
                                    .AppendLine($"<{localName}>")
                                    .Append(content)
                                    .AppendLine()
                                    .AppendLine($"</{localName}>")
                                    .Return();
        }

        private static string CreateElementXml(string content, string localName, string nameArgumentValue)
        {
            if (content.IndexOf('\n') < 0)
            {
                return $"<{localName} name=\"{nameArgumentValue}\">{content}</{localName}>";
            }

            return StringBuilderPool.Borrow()
                                    .AppendLine($"<{localName} name=\"{nameArgumentValue}\">")
                                    .Append(content)
                                    .AppendLine()
                                    .AppendLine($"</{localName}>")
                                    .Return();
        }

        private static string LeadingWhitespace(this DocumentationCommentTriviaSyntax comment)
        {
            return comment.ParentTrivia.Token.LeadingTrivia.TryFirst(x => x.IsKind(SyntaxKind.WhitespaceTrivia), out var whitespaceTrivia)
                ? whitespaceTrivia.ToString()
                : "        ";
        }
    }
}

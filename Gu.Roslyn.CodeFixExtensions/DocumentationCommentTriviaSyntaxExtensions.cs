namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for <see cref="DocumentationCommentTriviaSyntax"/>
    /// </summary>
    public static class DocumentationCommentTriviaSyntaxExtensions
    {
        public static DocumentationCommentTriviaSyntax WithSummary(this DocumentationCommentTriviaSyntax comment, string text)
        {
            return comment.WithSummary(text.IndexOf('\n') < 0
                ? Parse.XmlElementSyntax($"<summary> {text} </summary>")
                : Parse.XmlElementSyntax($"<summary> \r\n {text} \r\n </summary>"));
        }

        public static DocumentationCommentTriviaSyntax WithSummary(this DocumentationCommentTriviaSyntax comment, XmlElementSyntax summary)
        {
            if (comment.TryGetSummary(out var old))
            {
                return comment.ReplaceNode(old, summary);
            }

            throw new NotImplementedException();
        }
    }
}

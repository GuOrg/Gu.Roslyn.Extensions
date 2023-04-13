namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for <see cref="DocumentationCommentTriviaSyntax"/>.
    /// </summary>
    public static class DocumentationCommentTriviaSyntaxExtensions
    {
        /// <summary>
        /// Get the summary element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/>.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the summary.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetSummary(this DocumentationCommentTriviaSyntax comment, [NotNullWhen(true)] out XmlElementSyntax? element)
        {
            if (comment is null)
            {
                throw new System.ArgumentNullException(nameof(comment));
            }

            element = null;
            return comment.Content.TrySingleOfType<XmlNodeSyntax, XmlElementSyntax>(x => x is { StartTag.Name.LocalName.Text: "summary" }, out element);
        }

        /// <summary>
        /// Get the summary element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/>.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the summary.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetReturns(this DocumentationCommentTriviaSyntax comment, [NotNullWhen(true)] out XmlElementSyntax? element)
        {
            if (comment is null)
            {
                throw new System.ArgumentNullException(nameof(comment));
            }

            element = null;
            return comment.Content.TrySingleOfType<XmlNodeSyntax, XmlElementSyntax>(x => x is { StartTag.Name.LocalName.Text: "returns" }, out element);
        }

        /// <summary>
        /// Get the &lt;param name="parameterName"&gt;The value to return.&lt;/param&gt; element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/>.</param>
        /// <param name="parameterName">The name of the parameter to search for.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the parameter.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetParam(this DocumentationCommentTriviaSyntax comment, string parameterName, [NotNullWhen(true)] out XmlElementSyntax? element)
        {
            if (comment is null)
            {
                throw new System.ArgumentNullException(nameof(comment));
            }

            if (parameterName is null)
            {
                throw new System.ArgumentNullException(nameof(parameterName));
            }

            element = null;
            return comment.Content.TrySingleOfType<XmlNodeSyntax, XmlElementSyntax>(x => x is { StartTag.Name.LocalName.Text: "param" } && x.HasNameAttribute(parameterName), out element);
        }

        /// <summary>
        /// Get the &lt;param name="parameterName"&gt;The value to return.&lt;/param&gt; element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/>.</param>
        /// <param name="parameterName">The name of the parameter to search for.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the parameter.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetTypeParam(this DocumentationCommentTriviaSyntax comment, string parameterName, [NotNullWhen(true)] out XmlElementSyntax? element)
        {
            if (comment is null)
            {
                throw new System.ArgumentNullException(nameof(comment));
            }

            if (parameterName is null)
            {
                throw new System.ArgumentNullException(nameof(parameterName));
            }

            element = null;
            return comment.Content.TrySingleOfType<XmlNodeSyntax, XmlElementSyntax>(x => x is { StartTag.Name.LocalName.Text: "typeparam" } && x.HasNameAttribute(parameterName), out element);
        }
    }
}

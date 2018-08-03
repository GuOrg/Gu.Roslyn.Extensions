namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for <see cref="DocumentationCommentTriviaSyntax"/>
    /// </summary>
    public static class DocumentationCommentTriviaSyntaxExtensions
    {
        /// <summary>
        /// Get the summary element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the summary.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetSummary(this DocumentationCommentTriviaSyntax comment, out XmlElementSyntax element)
        {
            element = null;
            return comment?.Content.TrySingleOfType(x => IsNamed(x, "summary"), out element) == true;
        }

        /// <summary>
        /// Get the summary element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the summary.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetReturns(this DocumentationCommentTriviaSyntax comment, out XmlElementSyntax element)
        {
            element = null;
            return comment?.Content.TrySingleOfType(x => IsNamed(x, "returns"), out element) == true;
        }

        /// <summary>
        /// Get the &lt;param name="parameterName"&gt;The value to return.&lt;/param&gt; element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="parameterName">The name of the parameter to search for.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the parameter.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetParam(this DocumentationCommentTriviaSyntax comment, string parameterName, out XmlElementSyntax element)
        {
            element = null;
            return comment?.Content.TrySingleOfType(x => IsNamed(x, "param") && x.HasNameAttribute(parameterName), out element) == true;
        }

        /// <summary>
        /// Get the &lt;param name="parameterName"&gt;The value to return.&lt;/param&gt; element if it exists.
        /// </summary>
        /// <param name="comment">The <see cref="DocumentationCommentTriviaSyntax"/></param>
        /// <param name="parameterName">The name of the parameter to search for.</param>
        /// <param name="element">The <see cref="XmlElementSyntax"/> for the parameter.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetTypeParam(this DocumentationCommentTriviaSyntax comment, string parameterName, out XmlElementSyntax element)
        {
            element = null;
            return comment?.Content.TrySingleOfType(x => IsNamed(x, "typeparam") && x.HasNameAttribute(parameterName), out element) == true;
        }

        /// <summary>
        /// Check if the tag name matches.
        /// Example:
        /// element.IsNamed("summary") returns true for: <summary>  Gets or sets the value </summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static bool IsNamed(this XmlElementSyntax element, string localName)
        {
            return element?.StartTag is XmlElementStartTagSyntax startTag &&
                   startTag.Name is XmlNameSyntax nameSyntax &&
                   nameSyntax.LocalName.ValueText == localName;
        }

        /// <summary>
        /// Check if the tag name matches.
        /// Example:
        /// element.HasNameAttribute("name") returns true for: &lt;param name="name"&gt;The value to return.&lt;/param&gt;
        /// </summary>
        /// <param name="element">Check if the element has name=<paramref name="name"/></param>
        /// <param name="name">The expected value of the name attribute</param>
        /// <returns>True if <paramref name="element"/> has a name attribute with value <paramref name="name"/> </returns>
        public static bool HasNameAttribute(this XmlElementSyntax element, string name)
        {
            return element?.StartTag is XmlElementStartTagSyntax startTag &&
                   startTag.Attributes.TrySingleOfType(out XmlNameAttributeSyntax attribute) &&
                   attribute.Identifier is IdentifierNameSyntax identifier &&
                   identifier.Identifier.ValueText == name;
        }
    }
}

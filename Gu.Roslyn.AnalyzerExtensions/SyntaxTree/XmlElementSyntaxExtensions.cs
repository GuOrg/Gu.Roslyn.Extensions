namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class XmlElementSyntaxExtensions
    {
        /// <summary>
        /// Try getting the local name from the start tag of the element.
        /// </summary>
        /// <param name="element">The <see cref="XmlElementSyntax"/></param>
        /// <param name="localName">The result if found.</param>
        /// <returns>True if a local name was found.</returns>
        public static bool TryGetLocalName(this XmlElementSyntax element, out string localName)
        {
            localName = null;
            if (element?.StartTag is XmlElementStartTagSyntax startTag &&
                startTag.Name is XmlNameSyntax nameSyntax)
            {
                localName = nameSyntax.LocalName.Text;
            }

            return localName != null;
        }

        /// <summary>
        /// Check if the tag name matches.
        /// Example:
        /// element.IsNamed("summary") returns true for: <summary>  Gets or sets the value </summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="localName"></param>
        /// <returns>True if the elements local name matches <paramref name="localName"/></returns>
        public static bool HasLocalName(this XmlElementSyntax element, string localName)
        {
            return element.TryGetLocalName(out var candidate) &&
                   candidate == localName;
        }

        /// <summary>
        /// Get the name attribute if it exists.
        /// </summary>
        /// <param name="element">The <see cref="XmlElementSyntax"/></param>
        /// <param name="attribute">The name attribute if it exits.</param>
        /// <returns>True if a name attribute was found.</returns>
        public static bool TryGetNameAttribute(this XmlElementSyntax element, out XmlNameAttributeSyntax attribute)
        {
            attribute = null;
            return element?.StartTag is XmlElementStartTagSyntax startTag &&
                   startTag.Attributes.TrySingleOfType(out attribute);
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
            return element.TryGetNameAttribute(out var attribute) &&
                   attribute.Identifier is IdentifierNameSyntax identifier &&
                   identifier.Identifier.ValueText == name;
        }
    }
}
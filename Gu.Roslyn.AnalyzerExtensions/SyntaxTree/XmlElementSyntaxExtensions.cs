namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="XmlElementSyntax"/>.
    /// </summary>
    public static class XmlElementSyntaxExtensions
    {
        /// <summary>
        /// Try getting the local name from the start tag of the element.
        /// </summary>
        /// <param name="element">The <see cref="XmlElementSyntax"/>.</param>
        /// <param name="localName">The result if found.</param>
        /// <returns>True if a local name was found.</returns>
        public static bool TryGetLocalName(this XmlElementSyntax element, [NotNullWhen(true)] out string? localName)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            if (element.StartTag is { Name: { LocalName: { Text: { } text } } })
            {
                localName = text;
                return true;
            }

            localName = null;
            return false;
        }

        /// <summary>
        /// Check if the tag name matches.
        /// Example:
        /// element.IsNamed("summary") returns true for:. <summary>  Gets or sets the value </summary>
        /// </summary>
        /// <param name="element">The <see cref="XmlElementSyntax"/>.</param>
        /// <param name="localName">The name of the element.</param>
        /// <returns>True if the elements local name matches <paramref name="localName"/>.</returns>
        public static bool HasLocalName(this XmlElementSyntax element, string localName)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return element.TryGetLocalName(out var candidate) &&
                   candidate == localName;
        }

        /// <summary>
        /// Get the name attribute if it exists.
        /// </summary>
        /// <param name="element">The <see cref="XmlElementSyntax"/>.</param>
        /// <param name="attribute">The name attribute if it exits.</param>
        /// <returns>True if a name attribute was found.</returns>
        public static bool TryGetNameAttribute(this XmlElementSyntax element, [NotNullWhen(true)] out XmlNameAttributeSyntax? attribute)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            if (element.StartTag is { Attributes: { } attributes })
            {
                return attributes.TrySingleOfType<XmlAttributeSyntax, XmlNameAttributeSyntax>(out attribute);
            }

            attribute = null;
            return false;
        }

        /// <summary>
        /// Check if the tag name matches.
        /// Example:
        /// element.HasNameAttribute("name") returns true for: &lt;param name="name"&gt;The value to return.&lt;/param&gt;.
        /// </summary>
        /// <param name="element">Check if the element has name=<paramref name="name"/>.</param>
        /// <param name="name">The expected value of the name attribute.</param>
        /// <returns>True if <paramref name="element"/> has a name attribute with value <paramref name="name"/>. </returns>
        public static bool HasNameAttribute(this XmlElementSyntax element, string name)
        {
            return element.TryGetNameAttribute(out var attribute) &&
                   attribute.Identifier is { Identifier: { ValueText: { } valueText } } &&
                   valueText == name;
        }
    }
}

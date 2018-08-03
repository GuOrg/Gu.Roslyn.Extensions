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
            return comment?.Content.TrySingleOfType(x => x.HasLocalName("summary"), out element) == true;
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
            return comment?.Content.TrySingleOfType(x => x.HasLocalName("returns"), out element) == true;
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
            return comment?.Content.TrySingleOfType(x => x.HasLocalName("param") && 
                                                         x.HasNameAttribute(parameterName), out element) == true;
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
            return comment?.Content.TrySingleOfType(x => x.HasLocalName("typeparam") && 
                                                         x.HasNameAttribute(parameterName), out element) == true;
        }
    }
}

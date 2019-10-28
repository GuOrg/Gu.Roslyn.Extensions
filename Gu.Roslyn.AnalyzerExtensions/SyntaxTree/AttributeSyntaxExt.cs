namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="AttributeSyntax"/>.
    /// </summary>
    public static class AttributeSyntaxExt
    {
        /// <summary>
        /// Find the single argument.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/>.</param>
        /// <param name="argument">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TrySingleArgument(this AttributeSyntax attribute, out AttributeArgumentSyntax argument)
        {
            var argumentList = attribute?.ArgumentList;
            if (argumentList == null)
            {
                argument = null;
                return false;
            }

            return argumentList.Arguments.TrySingle(out argument);
        }

        /// <summary>
        /// Find argument by name or index.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/>.</param>
        /// <param name="index">The index.</param>
        /// <param name="name">The name colon or name equals name.</param>
        /// <param name="argument">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TryFindArgument(this AttributeSyntax attribute, int index, string name, out AttributeArgumentSyntax argument)
        {
            if (attribute == null)
            {
                throw new System.ArgumentNullException(nameof(attribute));
            }

            if (attribute.ArgumentList is AttributeArgumentListSyntax argumentList)
            {
                foreach (var candidate in attribute.ArgumentList.Arguments)
                {
                    if (candidate.NameColon is NameColonSyntax nameColon &&
                        nameColon.Name.Identifier.ValueText == name)
                    {
                        argument = candidate;
                        return true;
                    }

                    if (candidate.NameEquals is NameEqualsSyntax nameEquals &&
                        nameEquals.Name.Identifier.ValueText == name)
                    {
                        argument = candidate;
                        return true;
                    }
                }

                return argumentList.Arguments.TryElementAt(index, out argument) &&
                       argument.NameColon is null &&
                       argument.NameEquals is null;
            }

            argument = null;
            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
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
        public static bool TrySingleArgument(this AttributeSyntax attribute, [NotNullWhen(true)] out AttributeArgumentSyntax? argument)
        {
            if (attribute is null)
            {
                throw new System.ArgumentNullException(nameof(attribute));
            }

            argument = null;
            return attribute is { ArgumentList: { Arguments: { } arguments } } &&
                   arguments.TrySingle(out argument);
        }

        /// <summary>
        /// Find argument by name or index.
        /// </summary>
        /// <param name="attribute">The <see cref="AttributeSyntax"/>.</param>
        /// <param name="index">The index.</param>
        /// <param name="name">The name colon or name equals name.</param>
        /// <param name="argument">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TryFindArgument(this AttributeSyntax attribute, int index, string name, [NotNullWhen(true)] out AttributeArgumentSyntax? argument)
        {
            if (attribute is null)
            {
                throw new System.ArgumentNullException(nameof(attribute));
            }

            if (attribute.ArgumentList is { Arguments: { } arguments })
            {
                foreach (var candidate in arguments)
                {
                    switch (candidate)
                    {
                        case { NameColon: { Name: { Identifier: { } nameColon } } }
                            when nameColon.ValueText == name:
                        case { NameEquals: { Name: { Identifier: { } nameEquals } } }
                            when nameEquals.ValueText == name:
                            argument = candidate;
                            return true;
                    }
                }

                return arguments.TryElementAt(index, out argument) &&
                       argument.NameColon is null &&
                       argument.NameEquals is null;
            }

            argument = null;
            return false;
        }
    }
}

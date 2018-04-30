namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ArgumentListSyntax"/>
    /// </summary>
    public static class ArgumentListSyntaxExt
    {
        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="argumentList">The <see cref="ObjectCreationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFind(this ArgumentListSyntax argumentList, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            argument = null;
            if (argumentList == null ||
                argumentList.Arguments.Count == 0 ||
                parameter == null ||
                parameter.IsParams)
            {
                return false;
            }

            if (TryFindByNameColon(argumentList, parameter.Name, out argument))
            {
                return true;
            }

            return argumentList.Arguments.TryElementAt(parameter.Ordinal, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="name"/>
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/></param>
        /// <param name="name">The name</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindByNameColon(this ArgumentListSyntax argumentList, string name, out ArgumentSyntax argument)
        {
            argument = null;
            if (argumentList == null)
            {
                return false;
            }

            foreach (var candidate in argumentList.Arguments)
            {
                if (candidate.NameColon is NameColonSyntax nameColon &&
                    nameColon.Name.Identifier.ValueText == name)
                {
                    argument = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}

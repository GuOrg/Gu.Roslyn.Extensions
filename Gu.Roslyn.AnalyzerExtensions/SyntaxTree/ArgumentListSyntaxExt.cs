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
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetMatchingArgument(this InvocationExpressionSyntax invocation, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            return TryGetMatchingArgument(invocation?.ArgumentList, parameter, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="objectCreation">The <see cref="ObjectCreationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetMatchingArgument(this ObjectCreationExpressionSyntax objectCreation, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            return TryGetMatchingArgument(objectCreation?.ArgumentList, parameter, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="initializer">The <see cref="ConstructorInitializerSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetMatchingArgument(this ConstructorInitializerSyntax initializer, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            return TryGetMatchingArgument(initializer?.ArgumentList, parameter, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="argumentList">The <see cref="ObjectCreationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryGetMatchingArgument(this ArgumentListSyntax argumentList, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            argument = null;
            if (argumentList == null ||
                argumentList.Arguments.Count == 0 ||
                parameter == null ||
                parameter.IsParams)
            {
                return false;
            }

            if (TryGetArgumentByNameColon(argumentList, parameter.Name, out argument))
            {
                return true;
            }

            return argumentList.Arguments.TryElementAt(parameter.Ordinal, out argument);
        }

        private static bool TryGetArgumentByNameColon(this ArgumentListSyntax argumentList, string name, out ArgumentSyntax argument)
        {
            foreach (var candidate in argumentList.Arguments)
            {
                if (candidate.NameColon is NameColonSyntax nameColon &&
                    nameColon.Name.Identifier.ValueText == name)
                {
                    argument = candidate;
                    return true;
                }
            }

            argument = null;
            return false;
        }
    }
}

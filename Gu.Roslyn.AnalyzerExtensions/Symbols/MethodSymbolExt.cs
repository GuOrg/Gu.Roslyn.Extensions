namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="IMethodSymbol"/>
    /// </summary>
    public static class MethodSymbolExt
    {
        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/></param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryGetMatchingParameter(this IMethodSymbol method, ArgumentSyntax argument, out IParameterSymbol parameter)
        {
            if (argument.NameColon is NameColonSyntax nameColon &&
                nameColon.Name is IdentifierNameSyntax name)
            {
                return method.Parameters.TrySingle(x => x.Name == name.Identifier.ValueText, out parameter);
            }

            if (argument.Parent is ArgumentListSyntax argumentList)
            {
                return method.Parameters.TryElementAt(argumentList.Arguments.IndexOf(argument), out parameter);
            }

            parameter = null;
            return false;
        }
    }
}

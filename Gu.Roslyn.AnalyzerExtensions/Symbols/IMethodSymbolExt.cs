namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="IMethodSymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IMethodSymbolExt
    {
        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/>.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this IMethodSymbol method, ArgumentSyntax argument, out IParameterSymbol parameter)
        {
            if (argument.NameColon is NameColonSyntax nameColon &&
                nameColon.Name is IdentifierNameSyntax name)
            {
                return method.TryFindParameter(name.Identifier.ValueText, out parameter);
            }

            if (argument.Parent is ArgumentListSyntax argumentList)
            {
                return method.Parameters.TryElementAt(argumentList.Arguments.IndexOf(argument), out parameter);
            }

            parameter = null;
            return false;
        }

        /// <summary>
        /// Find the parameter by name.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/>.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this IMethodSymbol method, string name, out IParameterSymbol parameter)
        {
            parameter = null;
            if (method == null)
            {
                return false;
            }

            foreach (var candidate in method.Parameters)
            {
                if (candidate.Name == name)
                {
                    parameter = candidate;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find the parameter by type.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/>.</param>
        /// <param name="type">The type fo the parameter.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this IMethodSymbol method, QualifiedType type, out IParameterSymbol parameter)
        {
            parameter = null;
            if (method == null)
            {
                return false;
            }

            foreach (var candidate in method.Parameters)
            {
                if (candidate.Type == type)
                {
                    parameter = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}

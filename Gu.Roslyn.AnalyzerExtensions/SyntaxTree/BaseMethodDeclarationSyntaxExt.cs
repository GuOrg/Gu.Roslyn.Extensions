namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="BaseMethodDeclarationSyntax"/>
    /// </summary>
    public static class BaseMethodDeclarationSyntaxExt
    {
        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="method">The <see cref="BaseMethodDeclarationSyntax"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/></param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this BaseMethodDeclarationSyntax method, ArgumentSyntax argument, out ParameterSyntax parameter)
        {
            parameter = null;
            if (argument == null ||
                method == null)
            {
                return false;
            }

            if (method.ParameterList is ParameterListSyntax parameterList)
            {
                if (argument.NameColon is NameColonSyntax nameColon)
                {
                    return parameterList.TryFind(nameColon.Name.Identifier.ValueText, out parameter);
                }

                if (argument.Parent is ArgumentListSyntax argumentList)
                {
                    var index = argumentList.Arguments.IndexOf(argument);
                    if (method.ParameterList.Parameters.TryElementAt(index, out parameter))
                    {
                        return true;
                    }

                    parameter = method.ParameterList.Parameters.Last();
                    foreach (var modifier in parameter.Modifiers)
                    {
                        if (modifier.IsKind(SyntaxKind.ParamsKeyword))
                        {
                            return true;
                        }
                    }

                    parameter = null;
                    return false;
                }
            }

            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="BaseMethodDeclarationSyntax"/>.
    /// </summary>
    public static class BaseMethodDeclarationSyntaxExt
    {
        /// <summary>
        /// Get the <see cref="Accessibility"/> from the modifiers.
        /// </summary>
        /// <param name="declaration">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="Accessibility"/>.</returns>
        public static Accessibility Accessibility(this BaseMethodDeclarationSyntax declaration)
        {
            if (declaration is null)
            {
                return Microsoft.CodeAnalysis.Accessibility.NotApplicable;
            }

            if (declaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Private;
            }

            if (declaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            if (declaration.Modifiers.Any(SyntaxKind.ProtectedKeyword) &&
                declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal;
            }

            if (declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Internal;
            }

            if (declaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Protected;
            }

            if (declaration is MethodDeclarationSyntax methodDeclaration &&
                methodDeclaration.ExplicitInterfaceSpecifier != null)
            {
                // This will not always be right.
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            return Microsoft.CodeAnalysis.Accessibility.Internal;
        }

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="method">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this BaseMethodDeclarationSyntax method, ArgumentSyntax argument, [NotNullWhen(true)]out ParameterSyntax? parameter)
        {
            parameter = null;
            if (argument is null ||
                method is null)
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

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="method">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/>.</param>
        /// <returns>True if a matching parameter was found.</returns>
        public static bool TryFindParameter(this BaseMethodDeclarationSyntax method, string name, [NotNullWhen(true)]out ParameterSyntax? parameter)
        {
            parameter = null;
            if (name is null ||
                method is null)
            {
                return false;
            }

            if (method.ParameterList is ParameterListSyntax parameterList)
            {
                foreach (var candidate in parameterList.Parameters)
                {
                    if (candidate.Identifier.ValueText == name)
                    {
                        parameter = candidate;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

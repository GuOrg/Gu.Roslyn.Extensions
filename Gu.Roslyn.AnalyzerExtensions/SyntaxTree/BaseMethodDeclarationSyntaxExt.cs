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
                throw new System.ArgumentNullException(nameof(declaration));
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

            if (declaration is MethodDeclarationSyntax { ExplicitInterfaceSpecifier: { } })
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
        public static bool TryFindParameter(this BaseMethodDeclarationSyntax method, ArgumentSyntax argument, [NotNullWhen(true)] out ParameterSyntax? parameter)
        {
            if (method is null)
            {
                throw new System.ArgumentNullException(nameof(method));
            }

            if (argument is null)
            {
                throw new System.ArgumentNullException(nameof(argument));
            }

            parameter = null;

            if (method.ParameterList is { Parameters: { } parameters })
            {
                if (argument.NameColon is { Name.Identifier: { } name })
                {
                    foreach (var candidate in parameters)
                    {
                        if (candidate.Identifier.ValueText == name.ValueText)
                        {
                            parameter = candidate;
                            return true;
                        }
                    }

                    return false;
                }

                if (argument.Parent is ArgumentListSyntax { Arguments: { } arguments })
                {
                    var index = arguments.IndexOf(argument);
                    if (parameters.TryElementAt(index, out parameter))
                    {
                        return true;
                    }

                    parameter = parameters.Last();
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
        public static bool TryFindParameter(this BaseMethodDeclarationSyntax method, string name, [NotNullWhen(true)] out ParameterSyntax? parameter)
        {
            parameter = null;
            if (name is null ||
                method is null)
            {
                return false;
            }

            if (method.ParameterList is { Parameters: { } parameters })
            {
                foreach (var candidate in parameters)
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

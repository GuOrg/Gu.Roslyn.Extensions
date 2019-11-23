namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="FieldDeclarationSyntax"/>.
    /// </summary>
    public static class FieldDeclarationSyntaxExt
    {
        /// <summary>
        /// Get the <see cref="Microsoft.CodeAnalysis.Accessibility"/> from the modifiers.
        /// </summary>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="Microsoft.CodeAnalysis.Accessibility"/>.</returns>
        public static Accessibility Accessibility(this FieldDeclarationSyntax declaration)
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

            return Microsoft.CodeAnalysis.Accessibility.Private;
        }

        /// <summary>
        /// Try getting the name of the field.
        /// </summary>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <returns>True if the declaration is for a single variable.</returns>
        public static bool TryGetName(this FieldDeclarationSyntax declaration, [NotNullWhen(true)] out string? name)
        {
            name = null;
            if (declaration is null)
            {
                return false;
            }

            if (declaration.Declaration is { } variableDeclaration &&
                variableDeclaration.Variables.TrySingle(out var variable))
            {
                name = variable.Identifier.ValueText;
            }

            return name != null;
        }

        /// <summary>
        /// Try to find the variable by name.
        /// </summary>
        /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <param name="result">The match.</param>
        /// <returns>True if a match as found.</returns>
        public static bool TryFindVariable(this FieldDeclarationSyntax fieldDeclaration, string name, [NotNullWhen(true)] out VariableDeclaratorSyntax? result)
        {
            result = null;
            if (fieldDeclaration is null)
            {
                return false;
            }

            foreach (var candidate in fieldDeclaration.Declaration.Variables)
            {
                if (candidate.Identifier.ValueText == name)
                {
                    result = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}

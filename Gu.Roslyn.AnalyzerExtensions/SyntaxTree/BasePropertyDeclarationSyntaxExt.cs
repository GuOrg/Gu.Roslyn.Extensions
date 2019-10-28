namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="BasePropertyDeclarationSyntax"/>.
    /// </summary>
    public static class BasePropertyDeclarationSyntaxExt
    {
        /// <summary>
        /// Get the <see cref="Microsoft.CodeAnalysis.Accessibility"/> from the modifiers.
        /// </summary>
        /// <param name="declaration">The <see cref="BaseMethodDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="Microsoft.CodeAnalysis.Accessibility"/>.</returns>
        public static Accessibility Accessibility(this BasePropertyDeclarationSyntax declaration)
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

            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                // This will not always be right.
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            return Microsoft.CodeAnalysis.Accessibility.Internal;
        }

        /// <summary>
        /// Tries to get the get accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="result">The get accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetGetter(this BasePropertyDeclarationSyntax property, [NotNullWhen(true)]out AccessorDeclarationSyntax? result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.GetAccessorDeclaration), out result) == true;
        }

        /// <summary>
        /// Tries to get the set accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="result">The set accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetSetter(this BasePropertyDeclarationSyntax property, [NotNullWhen(true)]out AccessorDeclarationSyntax? result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.SetAccessorDeclaration), out result) == true;
        }

        /// <summary>
        /// Returns true if the get accessor exists and odes not have a body nor expression body and set accessor does not exist.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsGetOnly(this BasePropertyDeclarationSyntax property)
        {
            return property.TryGetGetter(out var getter) &&
                   getter.Body is null &&
                   getter.ExpressionBody is null &&
                   !property.TryGetSetter(out _);
        }

        /// <summary>
        /// Returns true if the get and set accessor if it exists has no body nor expression body.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsAutoProperty(this BasePropertyDeclarationSyntax property)
        {
            if (property.TryGetGetter(out var getter) &&
                getter.Body is null &&
                getter.ExpressionBody is null)
            {
                if (property.TryGetSetter(out var setter))
                {
                    return setter.Body is null &&
                           setter.ExpressionBody is null;
                }

                return true;
            }

            return false;
        }
    }
}

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
        /// <param name="declaration">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="Microsoft.CodeAnalysis.Accessibility"/>.</returns>
        public static Accessibility Accessibility(this BasePropertyDeclarationSyntax declaration)
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

            if (declaration.ExplicitInterfaceSpecifier is { })
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
        /// <returns><see cref="AccessorDeclarationSyntax"/> if <paramref name="property"/> has a get accessor.</returns>
        public static AccessorDeclarationSyntax? Getter(this BasePropertyDeclarationSyntax property)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return property.AccessorList?.Accessors.FirstOrDefault(SyntaxKind.GetAccessorDeclaration);
        }

        /// <summary>
        /// Tries to get the get accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns><see cref="AccessorDeclarationSyntax"/> if <paramref name="property"/> has a get accessor.</returns>
        public static AccessorDeclarationSyntax? Setter(this BasePropertyDeclarationSyntax property)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return property.AccessorList?.Accessors.FirstOrDefault(SyntaxKind.SetAccessorDeclaration);
        }

        /// <summary>
        /// Tries to get the get accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="result">The get accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetGetter(this BasePropertyDeclarationSyntax property, [NotNullWhen(true)] out AccessorDeclarationSyntax? result)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            result = null;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return property.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.GetAccessorDeclaration), out result) == true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }

        /// <summary>
        /// Tries to get the set accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <param name="result">The set accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetSetter(this BasePropertyDeclarationSyntax property, [NotNullWhen(true)] out AccessorDeclarationSyntax? result)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            result = null;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return property.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.SetAccessorDeclaration), out result) == true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }

        /// <summary>
        /// Returns true if the get accessor exists and odes not have a body nor expression body and set accessor does not exist.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsGetOnly(this BasePropertyDeclarationSyntax property)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return property is { AccessorList: { Accessors: { Count: 1 } accessors } } &&
                   accessors[0] is { Body: null, ExpressionBody: null } accessor &&
                   accessor.IsKind(SyntaxKind.GetAccessorDeclaration);
        }

        /// <summary>
        /// Returns true if the get and set accessor if it exists has no body nor expression body.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsAutoProperty(this BasePropertyDeclarationSyntax property)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            if (property.TryGetGetter(out var getter) &&
                getter is { Body: null, ExpressionBody: null })
            {
                if (property.TryGetSetter(out var setter))
                {
                    return setter is { Body: null, ExpressionBody: null };
                }

                return true;
            }

            return false;
        }
    }
}

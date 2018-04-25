namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="BasePropertyDeclarationSyntax"/>
    /// </summary>
    public static class BasePropertyDeclarationSyntaxExt
    {
        /// <summary>
        /// Tries to get the get accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/></param>
        /// <param name="result">The get accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetGetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.GetAccessorDeclaration), out result) == true;
        }

        /// <summary>
        /// Tries to get the set accessor.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/></param>
        /// <param name="result">The set accessor if found.</param>
        /// <returns>True if <paramref name="property"/> has a get accessor.</returns>
        public static bool TryGetSetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.SetAccessorDeclaration), out result) == true;
        }

        /// <summary>
        /// Returns true if the get accessor exists and odes not have a body nor expression body and set accessor does not exist.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/></param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsGetOnly(this BasePropertyDeclarationSyntax property)
        {
            if (property.TryGetGetter(out var getter) &&
                getter.Body == null &&
                getter.ExpressionBody == null)
            {
                return !property.TryGetSetter(out _);
            }

            return false;
        }

        /// <summary>
        /// Returns true if the get and set accessor if it exists has no body nor expression body.
        /// </summary>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/></param>
        /// <returns>True if <paramref name="property"/> is an auto property.</returns>
        public static bool IsAutoProperty(this BasePropertyDeclarationSyntax property)
        {
            if (property.TryGetGetter(out var getter) &&
                getter.Body == null &&
                getter.ExpressionBody == null)
            {
                if (property.TryGetSetter(out var setter))
                {
                    return setter.Body == null &&
                           setter.ExpressionBody == null;
                }

                return true;
            }

            return false;
        }
    }
}

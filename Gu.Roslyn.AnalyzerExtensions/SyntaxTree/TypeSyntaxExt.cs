namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="TypeSyntax"/>.
    /// </summary>
    public static class TypeSyntaxExt
    {
        /// <summary>
        /// Check if the type is void.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if void.</returns>
        public static bool IsVoid(this TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefinedType)
            {
                return predefinedType.Keyword.ValueText == "void";
            }

            return false;
        }
    }
}

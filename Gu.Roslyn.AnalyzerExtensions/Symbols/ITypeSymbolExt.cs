namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for working with <see cref="ITypeSymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ITypeSymbolExt
    {
        /// <summary>
        /// Check if <paramref name="type"/> has a single type argument.
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/>.</param>
        /// <param name="typeArgument">The single type argument.</param>
        /// <returns>True if <paramref name="type"/> has a single type argument.</returns>
        public static bool TryGetSingleTypeArgument(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            typeArgument = null;
            return type is INamedTypeSymbol namedType &&
                   namedType.IsGenericType &&
                   namedType.TypeArguments.TrySingle(out typeArgument);
        }
    }
}

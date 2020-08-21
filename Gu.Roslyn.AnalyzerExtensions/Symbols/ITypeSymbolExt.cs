namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
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
        public static bool TryGetSingleTypeArgument(this ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? typeArgument)
        {
            typeArgument = null;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return type is INamedTypeSymbol { IsGenericType: true, TypeArguments: { Length: 1 } typeArguments } &&
                   typeArguments.TrySingle(out typeArgument);
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }
    }
}

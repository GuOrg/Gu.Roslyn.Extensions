namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helper methods for working with <see cref="ISymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ISymbolExt
    {
        /// <summary>
        /// Check if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="kind1">The first type to check for.</param>
        /// <param name="kind2">The second type to check for.</param>
        /// <returns>True if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/>.</returns>
        public static bool IsEitherKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2) => symbol != null && (symbol.Kind == kind1 || symbol.Kind == kind2);

        /// <summary>
        /// Check if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/> or <paramref name="kind3"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="kind1">The first type to check for.</param>
        /// <param name="kind2">The second type to check for.</param>
        /// <param name="kind3">The third type to check for.</param>
        /// <returns>True if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/> or <paramref name="kind3"/>.</returns>
        public static bool IsEitherKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3) => symbol != null && (symbol.Kind == kind1 || symbol.Kind == kind2 || symbol.Kind == kind3);

        /// <summary>
        /// Check if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The first type to check for.</typeparam>
        /// <typeparam name="T2">The second type to check for.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/>.</returns>
        public static bool IsEither<T1, T2>(this ISymbol symbol)
            where T1 : ISymbol
            where T2 : ISymbol
        {
            return symbol is T1 || symbol is T2;
        }

        /// <summary>
        /// Check if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/> or <typeparamref name="T3"/>.
        /// </summary>
        /// <typeparam name="T1">The first type to check for.</typeparam>
        /// <typeparam name="T2">The second type to check for.</typeparam>
        /// <typeparam name="T3">The third type to check for.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/> or <typeparamref name="T3"/>.</returns>
        public static bool IsEither<T1, T2, T3>(this ISymbol symbol)
            where T1 : ISymbol
            where T2 : ISymbol
            where T3 : ISymbol
        {
            return symbol is T1 || symbol is T2 || symbol is T3;
        }

        /// <summary>
        /// Check if the symbols are
        /// - equal
        /// - x is equal to the definition of y
        /// - x is equal to an overridden y if property.
        /// </summary>
        /// <param name="x">The first symbol.</param>
        /// <param name="y">The second symbol.</param>
        /// <returns>True if the symbols are found equivalent.</returns>
        public static bool IsEquivalentTo(this ISymbol x, ISymbol y)
        {
            if (x == null ||
                y == null ||
                x.Name != y.Name ||
                x.Kind != y.Kind)
            {
                return false;
            }

            if (x.Equals(y))
            {
                return true;
            }

            if (x.IsDefinition &&
                !y.Equals(y.OriginalDefinition))
            {
                return IsEquivalentTo(x, y.OriginalDefinition);
            }

            switch (y)
            {
                case IParameterSymbol _:
                    return IsEquivalentTo(x.ContainingSymbol, y.ContainingSymbol);
                case IPropertySymbol property when property.OverriddenProperty is IPropertySymbol overridden:
                    return IsEquivalentTo(x, overridden);
                case IEventSymbol eventSymbol when eventSymbol.OverriddenEvent is IEventSymbol overridden:
                    return IsEquivalentTo(x, overridden);
                case IMethodSymbol methodSymbol when methodSymbol.OverriddenMethod is IMethodSymbol overridden:
                    return IsEquivalentTo(x, overridden);
                case IMethodSymbol ym when x is IMethodSymbol xm && xm.IsExtensionMethod && ym.IsExtensionMethod:
                    return xm.ReducedFrom?.Equals(ym) == true ||
                           xm.Equals(ym.ReducedFrom);
            }

            return false;
        }

        /// <summary>
        /// Try get the single attribute of type <paramref name="attributeType"/> declared on <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="attributeType">The <see cref="QualifiedType"/>.</param>
        /// <param name="attribute">The attribute if found.</param>
        /// <returns>True if a single attribute of type <paramref name="attributeType"/> declared on <paramref name="symbol"/>.</returns>
        public static bool TryGetAttribute(this ISymbol symbol, QualifiedType attributeType, out AttributeData attribute)
        {
            return symbol.GetAttributes().TrySingle(x => x.AttributeClass == attributeType, out attribute);
        }

        /// <summary>
        /// Check if <paramref name="symbol"/> has [System.CodeDom.Compiler.GeneratedCodeAttribute].
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if the attribute is defined on the symbol.</returns>
        public static bool HasGeneratedCodeAttribute(this ISymbol symbol) => symbol.TryGetAttribute(QualifiedType.System.CodeDom.Compiler.GeneratedCodeAttribute, out _);

        /// <summary>
        /// Check if <paramref name="symbol"/> has [System.Runtime.CompilerServices.CompilerGeneratedAttribute].
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if the attribute is defined on the symbol.</returns>
        public static bool HasCompilerGeneratedAttribute(this ISymbol symbol) => symbol.TryGetAttribute(QualifiedType.System.Runtime.CompilerServices.CompilerGeneratedAttribute, out _);
    }
}

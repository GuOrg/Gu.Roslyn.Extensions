namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
        public static bool IsEitherKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2) => symbol is { } && (symbol.Kind == kind1 || symbol.Kind == kind2);

        /// <summary>
        /// Check if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/> or <paramref name="kind3"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="kind1">The first type to check for.</param>
        /// <param name="kind2">The second type to check for.</param>
        /// <param name="kind3">The third type to check for.</param>
        /// <returns>True if <paramref name="symbol"/> is either <paramref name="kind1"/> or <paramref name="kind2"/> or <paramref name="kind3"/>.</returns>
        public static bool IsEitherKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3) => symbol is { } && (symbol.Kind == kind1 || symbol.Kind == kind2 || symbol.Kind == kind3);

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
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            return Name() ??
                   Kind() ??
                   OverriddenEvent(x as IEventSymbol, y as IEventSymbol) ??
                   OverriddenEvent(y as IEventSymbol, x as IEventSymbol) ??
                   OverriddenProperty(x as IPropertySymbol, y as IPropertySymbol) ??
                   OverriddenProperty(y as IPropertySymbol, x as IPropertySymbol) ??
                   OverriddenMethod(x as IMethodSymbol, y as IMethodSymbol) ??
                   OverriddenMethod(y as IMethodSymbol, x as IMethodSymbol) ??
                   ReducedFrom(x as IMethodSymbol, y as IMethodSymbol) ??
                   ReducedFrom(y as IMethodSymbol, x as IMethodSymbol) ??
                   Parameter(y as IParameterSymbol, x as IParameterSymbol) ??
                   Definition(x, y) ??
                   Definition(y, x) ??
                   SymbolComparer.Equal(x, y);

            bool? Name() => x.MetadataName == y.MetadataName ? null : false;

            bool? Kind() => x.Kind == y.Kind ? null : false;

            static bool? OverriddenEvent(IEventSymbol? x, IEventSymbol? y)
            {
                if (x is { OverriddenEvent: { } xOverridden })
                {
                    return y is not null && IsEquivalentTo(xOverridden, y);
                }

                return null;
            }

            static bool? OverriddenProperty(IPropertySymbol? x, IPropertySymbol? y)
            {
                if (x is { OverriddenProperty: { } xOverridden })
                {
                    return y is not null && IsEquivalentTo(xOverridden, y);
                }

                return null;
            }

            static bool? OverriddenMethod(IMethodSymbol? x, IMethodSymbol? y)
            {
                if (x is { OverriddenMethod: { } xOverridden })
                {
                    return y is not null && IsEquivalentTo(xOverridden, y);
                }

                return null;
            }

            static bool? ReducedFrom(IMethodSymbol? x, IMethodSymbol? y)
            {
                if (x is { ReducedFrom: { } xReducedFrom })
                {
                    return y is not null && IsEquivalentTo(xReducedFrom, y);
                }

                return null;
            }

            static bool? Parameter(IParameterSymbol? x, IParameterSymbol? y)
            {
                return (x, y) switch
                {
                    ({ Name: { } xName, ContainingSymbol: { } xContainingSymbol }, { Name: { } yName, ContainingSymbol: { } yContainingSymbol })
                        => xName == yName && IsEquivalentTo(xContainingSymbol, yContainingSymbol),
                    (_, _) => null,
                };
            }

            static bool? Definition(ISymbol x, ISymbol y)
            {
                return (x, y) switch
                {
                    ({ IsDefinition: true }, { IsDefinition: false, OriginalDefinition: { } yOriginalDefinition })
                        when !ReferenceEquals(y, yOriginalDefinition)
                        => IsEquivalentTo(x, yOriginalDefinition),
                    (_, _) => null,
                };
            }
        }

        /// <summary>
        /// Try get the single attribute of type <paramref name="attributeType"/> declared on <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="attributeType">The <see cref="QualifiedType"/>.</param>
        /// <param name="attribute">The attribute if found.</param>
        /// <returns>True if a single attribute of type <paramref name="attributeType"/> declared on <paramref name="symbol"/>.</returns>
        public static bool TryGetAttribute(this ISymbol symbol, QualifiedType attributeType, [NotNullWhen(true)] out AttributeData? attribute)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (attributeType is null)
            {
                throw new ArgumentNullException(nameof(attributeType));
            }

            attribute = null;
            return symbol.GetAttributes().TrySingle<AttributeData>(x => x.AttributeClass == attributeType, out attribute);
        }

        /// <summary>
        /// Check if <paramref name="symbol"/> has [System.CodeDom.Compiler.GeneratedCodeAttribute].
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if the attribute is defined on the symbol.</returns>
        public static bool HasGeneratedCodeAttribute(this ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return symbol.TryGetAttribute(QualifiedType.System.CodeDom.Compiler.GeneratedCodeAttribute, out _);
        }

        /// <summary>
        /// Check if <paramref name="symbol"/> has [System.Runtime.CompilerServices.CompilerGeneratedAttribute].
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <returns>True if the attribute is defined on the symbol.</returns>
        public static bool HasCompilerGeneratedAttribute(this ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return symbol.TryGetAttribute(QualifiedType.System.Runtime.CompilerServices.CompilerGeneratedAttribute, out _);
        }
    }
}

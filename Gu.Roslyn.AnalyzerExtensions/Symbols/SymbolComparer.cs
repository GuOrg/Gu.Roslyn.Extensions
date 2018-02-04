namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class SymbolComparer : IEqualityComparer<ISymbol>
    {
        internal static readonly SymbolComparer Default = new SymbolComparer();

        private SymbolComparer()
        {
        }

        public static bool Equals(ISymbol x, ISymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null ||
                y == null)
            {
                return false;
            }

            if (x is INamedTypeSymbol xNamed &&
                y is INamedTypeSymbol yNamed)
            {
                return NamedTypeSymbolComparer.Equals(xNamed, yNamed);
            }

            if (x is ITypeSymbol xType &&
                y is ITypeSymbol yType)
            {
                return TypeSymbolComparer.Equals(xType, yType);
            }

            if (x is IPropertySymbol xProp &&
                y is IPropertySymbol yProp)
            {
                return PropertySymbolComparer.Equals(xProp, yProp);
            }

            return x.Equals(y) ||
                   DefinitionEquals(x, y) ||
                   DefinitionEquals(y, x) ||
                   Equals((x as IPropertySymbol)?.OverriddenProperty, y) ||
                   Equals(x, (y as IPropertySymbol)?.OverriddenProperty) ||
                   Equals((x as IMethodSymbol)?.OverriddenMethod, y) ||
                   Equals(x, (y as IMethodSymbol)?.OverriddenMethod);
        }

        /// <inheritdoc/>
        bool IEqualityComparer<ISymbol>.Equals(ISymbol x, ISymbol y) => Equals(x, y);

        /// <inheritdoc/>
        public int GetHashCode(ISymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class SymbolComparer : IEqualityComparer<ISymbol>
    {
        public static readonly SymbolComparer Default = new SymbolComparer();

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

            if (x is INamedTypeSymbol xNamedType &&
                y is INamedTypeSymbol yNamedType)
            {
                return NamedTypeSymbolComparer.Equals(xNamedType, yNamedType);
            }

            if (x is ITypeSymbol xType &&
                y is ITypeSymbol yType)
            {
                return TypeSymbolComparer.Equals(xType, yType);
            }

            if (x is IFieldSymbol xField &&
                y is IFieldSymbol yField)
            {
                return FieldSymbolComparer.Equals(xField, yField);
            }

            if (x is IPropertySymbol xProperty &&
                y is IPropertySymbol yProperty)
            {
                return PropertySymbolComparer.Equals(xProperty, yProperty);
            }

            if (x is IMethodSymbol xMethod &&
                y is IMethodSymbol yMethod)
            {
                return MethodSymbolComparer.Equals(xMethod, yMethod);
            }

            if (x is ILocalSymbol xLocal &&
                y is ILocalSymbol yLocal)
            {
                return LocalSymbolComparer.Equals(xLocal, yLocal);
            }

            if (x is IParameterSymbol xParameter &&
                y is IParameterSymbol yParameter)
            {
                return ParameterSymbolComparer.Equals(xParameter, yParameter);
            }

            return x.Equals(y);
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

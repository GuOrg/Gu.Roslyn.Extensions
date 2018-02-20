namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class FieldSymbolComparer : IEqualityComparer<IFieldSymbol>
    {
        public static readonly FieldSymbolComparer Default = new FieldSymbolComparer();

        private FieldSymbolComparer()
        {
        }

        public static bool Equals(IFieldSymbol x, IFieldSymbol y)
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

            return x.MetadataName == y.MetadataName &&
                   NamedTypeSymbolComparer.Equals(x.ContainingType, y.ContainingType);
        }

        /// <inheritdoc />
        bool IEqualityComparer<IFieldSymbol>.Equals(IFieldSymbol x, IFieldSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IFieldSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

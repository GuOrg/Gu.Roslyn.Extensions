namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class EventSymbolComparer : IEqualityComparer<IEventSymbol>
    {
        internal static readonly EventSymbolComparer Default = new EventSymbolComparer();

        private EventSymbolComparer()
        {
        }

        public static bool Equals(IEventSymbol x, IEventSymbol y)
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
        bool IEqualityComparer<IEventSymbol>.Equals(IEventSymbol x, IEventSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IEventSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

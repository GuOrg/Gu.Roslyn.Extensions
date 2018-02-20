namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class PropertySymbolComparer : IEqualityComparer<IPropertySymbol>
    {
        public static readonly PropertySymbolComparer Default = new PropertySymbolComparer();

        private PropertySymbolComparer()
        {
        }

        public static bool Equals(IPropertySymbol x, IPropertySymbol y)
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
        bool IEqualityComparer<IPropertySymbol>.Equals(IPropertySymbol x, IPropertySymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IPropertySymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        // ReSharper disable once UnusedMember.Local
        [Obsolete("Should only be called with arguments of type IPropertySymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
    }
}

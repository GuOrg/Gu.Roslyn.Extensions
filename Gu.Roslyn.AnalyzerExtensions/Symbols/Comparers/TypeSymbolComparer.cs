namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
    {
        public static readonly TypeSymbolComparer Default = new TypeSymbolComparer();

        private TypeSymbolComparer()
        {
        }

        public static bool Equals(ITypeSymbol x, ITypeSymbol y)
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

            return x.MetadataName == y.MetadataName &&
                   x.ContainingNamespace?.MetadataName == y.ContainingNamespace?.MetadataName;
        }

        /// <inheritdoc />
        bool IEqualityComparer<ITypeSymbol>.Equals(ITypeSymbol x, ITypeSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(ITypeSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        // ReSharper disable once UnusedMember.Local
        [Obsolete("Should only be called with arguments of type ITypeSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
    }
}
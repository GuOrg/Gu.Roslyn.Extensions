namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class LocalSymbolComparer : IEqualityComparer<ILocalSymbol>
    {
        public static readonly LocalSymbolComparer Default = new LocalSymbolComparer();

        private LocalSymbolComparer()
        {
        }

        public static bool Equals(ILocalSymbol x, ILocalSymbol y)
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
                   SymbolComparer.Equals(x.ContainingSymbol, y.ContainingSymbol);
        }

        /// <inheritdoc />
        bool IEqualityComparer<ILocalSymbol>.Equals(ILocalSymbol x, ILocalSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(ILocalSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        // ReSharper disable once UnusedMember.Local
        [Obsolete("Should only be called with arguments of type ILocalSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
    }
}

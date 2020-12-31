namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class LocalSymbolComparer : IEqualityComparer<ILocalSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly LocalSymbolComparer Default = new LocalSymbolComparer();

        private LocalSymbolComparer()
        {
        }

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(ILocalSymbol? x, ILocalSymbol? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null ||
                y is null)
            {
                return false;
            }

            return x.MetadataName == y.MetadataName &&
                   SymbolComparer.Equal(x.ContainingSymbol, y.ContainingSymbol);
        }

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(ILocalSymbol? x, ILocalSymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
        [Obsolete("Should only be called with arguments of type ILocalSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<ILocalSymbol>.Equals(ILocalSymbol? x, ILocalSymbol? y) => Equal(x, y);

        /// <inheritdoc />
        public int GetHashCode(ILocalSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

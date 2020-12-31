namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class NamedTypeSymbolComparer : IEqualityComparer<INamedTypeSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly NamedTypeSymbolComparer Default = new NamedTypeSymbolComparer();

        private NamedTypeSymbolComparer()
        {
        }

        /// <summary> Determines equality by name, containing type, arity and namespace. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(INamedTypeSymbol? x, INamedTypeSymbol? y) => TypeSymbolComparer.Equal(x, y);

        /// <summary> Compares equality by name and containing type and treats overridden and definition as equal. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equivalent(INamedTypeSymbol? x, INamedTypeSymbol? y) => TypeSymbolComparer.Equal(x, y);

        /// <summary> Determines equality by name, containing type, arity and namespace. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(INamedTypeSymbol? x, INamedTypeSymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
        [Obsolete("Should only be called with arguments of type INamedTypeSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<INamedTypeSymbol>.Equals(INamedTypeSymbol? x, INamedTypeSymbol? y) => Equal(x, y);

        /// <inheritdoc/>
        public int GetHashCode(INamedTypeSymbol obj) => TypeSymbolComparer.GetHashCode(obj);
    }
}

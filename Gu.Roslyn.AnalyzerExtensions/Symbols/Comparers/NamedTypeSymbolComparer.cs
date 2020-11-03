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
        public static bool Equal(INamedTypeSymbol? x, INamedTypeSymbol? y)
        {
            if (x is { IsReferenceType: true })
            {
                return SymbolEqualityComparer.IncludeNullability.Equals(x, y);
            }

            return SymbolEqualityComparer.Default.Equals(x, y);
        }

        /// <summary> Determines equality by name, containing type, arity and namespace. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(INamedTypeSymbol? x, INamedTypeSymbol? y)
        {
            return Equal(x, y);
        }

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Obsolete("Should only be called with arguments of type INamedTypeSymbol.", error: true)]
#pragma warning disable IDE1006 // Naming Styles
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<INamedTypeSymbol>.Equals(INamedTypeSymbol? x, INamedTypeSymbol? y) => Equal(x, y);

        /// <inheritdoc/>
        public int GetHashCode(INamedTypeSymbol obj) => TypeSymbolComparer.GetHashCode(obj);
    }
}

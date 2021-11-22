namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class ParameterSymbolComparer : IEqualityComparer<IParameterSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly ParameterSymbolComparer Default = new();

        private ParameterSymbolComparer()
        {
        }

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(IParameterSymbol? x, IParameterSymbol? y)
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

        /// <summary> Compares equality by name and containing type and treats overridden and definition as equal. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equivalent(IParameterSymbol? x, IParameterSymbol? y)
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

            return Equal(x, y) ||
                   Definition(x, y);

            static bool Definition(IParameterSymbol x, IParameterSymbol y)
            {
                return x.Name == y.Name &&
                       SymbolComparer.Equivalent(x.ContainingSymbol, y.ContainingSymbol);
            }
        }

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(IParameterSymbol? x, IParameterSymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
        [Obsolete("Should only be called with arguments of type IParameterSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<IParameterSymbol>.Equals(IParameterSymbol? x, IParameterSymbol? y) => Equal(x, y);

        /// <inheritdoc />
        public int GetHashCode(IParameterSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

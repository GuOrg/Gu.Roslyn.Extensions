namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class ParameterSymbolComparer : IEqualityComparer<IParameterSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly ParameterSymbolComparer Default = new ParameterSymbolComparer();

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

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(IParameterSymbol? x, IParameterSymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Obsolete("Should only be called with arguments of type IParameterSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
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

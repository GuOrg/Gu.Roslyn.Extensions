namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class AssemblySymbolComparer : IEqualityComparer<IAssemblySymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly AssemblySymbolComparer Default = new();

        private AssemblySymbolComparer()
        {
        }

        /// <summary> Compares equality by name. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(IAssemblySymbol? x, IAssemblySymbol? y) => x?.Identity == y?.Identity;

        /// <summary> Compares equality by name. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(IAssemblySymbol? x, IAssemblySymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CA1707,CS1591,IDE1006,SA1313,SA1600
        [Obsolete("Should only be called with arguments of type IAssemblySymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CA1707,CS1591,IDE1006,SA1313,SA1600
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<IAssemblySymbol>.Equals(IAssemblySymbol? x, IAssemblySymbol? y) => Equal(x, y);

        /// <inheritdoc />
        public int GetHashCode(IAssemblySymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

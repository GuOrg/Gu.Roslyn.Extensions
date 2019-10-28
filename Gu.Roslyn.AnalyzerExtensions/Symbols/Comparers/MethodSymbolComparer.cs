namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class MethodSymbolComparer : IEqualityComparer<IMethodSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly MethodSymbolComparer Default = new MethodSymbolComparer();

        private MethodSymbolComparer()
        {
        }

        /// <summary> Determines equality by name, containing type, return type and parameters. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equals(IMethodSymbol x, IMethodSymbol y)
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
                   NamedTypeSymbolComparer.Equals(x.ContainingType, y.ContainingType) &&
                   TypeSymbolComparer.Equals(x.ReturnType, y.ReturnType) &&
                   ParametersMatches(x, y);
        }

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Obsolete("Should only be called with arguments of type IMethodSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<IMethodSymbol>.Equals(IMethodSymbol x, IMethodSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IMethodSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        private static bool ParametersMatches(IMethodSymbol x, IMethodSymbol y)
        {
            if (x.Parameters.Length != y.Parameters.Length)
            {
                return false;
            }

            for (var i = 0; i < x.Parameters.Length; i++)
            {
                if (!TypeSymbolComparer.Equals(x.Parameters[i].Type, y.Parameters[i].Type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

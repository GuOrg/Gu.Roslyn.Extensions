namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly TypeSymbolComparer Default = new TypeSymbolComparer();

        private TypeSymbolComparer()
        {
        }

        /// <summary> Determines equality by name, containing type, arity and namespace. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(ITypeSymbol? x, ITypeSymbol? y)
        {
            if (IsAnnotated(x) &&
                IsAnnotated(y))
            {
                return SymbolEqualityComparer.IncludeNullability.Equals(x, y);
            }

            return SymbolEqualityComparer.Default.Equals(x, y);

            static bool IsAnnotated(ITypeSymbol? type) => type is { IsReferenceType: true } &&
                                                          type.NullableAnnotation != NullableAnnotation.None;
        }

        /// <summary> Determines equality by name and containing symbol. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(ITypeSymbol? x, ITypeSymbol? y) => Equal(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Obsolete("Should only be called with arguments of type ITypeSymbol.", error: true)]
#pragma warning disable IDE1006 // Naming Styles
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

#pragma warning disable CA1720 // Identifier contains type name
        /// <summary>Returns the hash code for this string.</summary>
        /// <param name="obj">The instance.</param>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public static int GetHashCode(ITypeSymbol obj)
#pragma warning restore CA1720 // Identifier contains type name
        {
            if (obj is null)
            {
                return 0;
            }

            if (obj.TypeKind == TypeKind.TypeParameter)
            {
                return 1;
            }

            return obj.MetadataName.GetHashCode();
        }

        /// <inheritdoc />
        bool IEqualityComparer<ITypeSymbol>.Equals(ITypeSymbol? x, ITypeSymbol? y) => Equal(x, y);

        /// <inheritdoc />
        int IEqualityComparer<ITypeSymbol>.GetHashCode(ITypeSymbol obj)
        {
            return GetHashCode(obj);
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class SymbolComparer : IEqualityComparer<ISymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly SymbolComparer Default = new SymbolComparer();

        private SymbolComparer()
        {
        }

        /// <summary> Determines equality by delegating to other compare. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equal(ISymbol? x, ISymbol? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null ||
                y is null ||
                x.Kind != y.Kind ||
                x.MetadataName != y.MetadataName)
            {
                return false;
            }

            return FieldSymbolComparer.Equal(x as IFieldSymbol, y as IFieldSymbol) ||
                   EventSymbolComparer.Equal(x as IEventSymbol, y as IEventSymbol) ||
                   PropertySymbolComparer.Equal(x as IPropertySymbol, y as IPropertySymbol) ||
                   MethodSymbolComparer.Equal(x as IMethodSymbol, y as IMethodSymbol) ||
                   ParameterSymbolComparer.Equal(x as IParameterSymbol, y as IParameterSymbol) ||
                   LocalSymbolComparer.Equal(x as ILocalSymbol, y as ILocalSymbol) ||
                   NamedTypeSymbolComparer.Equal(x as INamedTypeSymbol, y as INamedTypeSymbol) ||
                   TypeSymbolComparer.Equal(x as ITypeSymbol, y as ITypeSymbol) ||
                   NamespaceSymbolComparer.Equal(x as INamespaceSymbol, y as INamespaceSymbol) ||
                   SymbolEqualityComparer.Default.Equals(x, y);
        }

        /// <summary> Determines equality by delegating to other compare. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equivalent(ISymbol? x, ISymbol? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null ||
                y is null ||
                x.Kind != y.Kind)
            {
                return false;
            }

            return FieldSymbolComparer.Equivalent(x as IFieldSymbol, y as IFieldSymbol) ||
                   EventSymbolComparer.Equivalent(x as IEventSymbol, y as IEventSymbol) ||
                   PropertySymbolComparer.Equivalent(x as IPropertySymbol, y as IPropertySymbol) ||
                   MethodSymbolComparer.Equivalent(x as IMethodSymbol, y as IMethodSymbol) ||
                   ParameterSymbolComparer.Equivalent(x as IParameterSymbol, y as IParameterSymbol) ||
                   LocalSymbolComparer.Equal(x as ILocalSymbol, y as ILocalSymbol) ||
                   NamedTypeSymbolComparer.Equivalent(x as INamedTypeSymbol, y as INamedTypeSymbol) ||
                   TypeSymbolComparer.Equivalent(x as ITypeSymbol, y as ITypeSymbol) ||
                   NamespaceSymbolComparer.Equal(x as INamespaceSymbol, y as INamespaceSymbol) ||
                   SymbolEqualityComparer.Default.Equals(x, y);
        }

        /// <summary> Determines equality by delegating to other compare. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        [Obsolete("Use Equal as RS1024 does not nag about it.")]
        public static bool Equals(ISymbol? x, ISymbol? y) => Equal(x, y);

        //// ReSharper disable UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(IEventSymbol _, IEventSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(IFieldSymbol _, IFieldSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(ILocalSymbol _, ILocalSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(IMethodSymbol _, IMethodSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(INamedTypeSymbol _, INamedTypeSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(INamespaceSymbol _, INamespaceSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(IParameterSymbol _, IParameterSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(IPropertySymbol _, IPropertySymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

        [Obsolete("Should only be called with arguments of type ISymbol.", error: true)]
        public static bool Equals(ITypeSymbol _, ITypeSymbol __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");

#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
        //// ReSharper restore UnusedMember.Global
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc/>
        bool IEqualityComparer<ISymbol>.Equals(ISymbol? x, ISymbol? y) => Equal(x, y);

        /// <inheritdoc/>
        public int GetHashCode(ISymbol obj)
        {
            if (obj is ITypeSymbol typeSymbol)
            {
                return TypeSymbolComparer.GetHashCode(typeSymbol);
            }

            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

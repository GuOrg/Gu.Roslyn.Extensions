namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    /// <inheritdoc />
    public sealed class SymbolComparer : IEqualityComparer<ISymbol>
    {
        /// <summary> The default instance. </summary>
        public static readonly SymbolComparer Default = new();

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

            return (x.Kind, y.Kind) switch
            {
                (SymbolKind.Field, SymbolKind.Field) => FieldSymbolComparer.Equal((IFieldSymbol)x, (IFieldSymbol)y),
                (SymbolKind.Event, SymbolKind.Event) => EventSymbolComparer.Equal((IEventSymbol)x, (IEventSymbol)y),
                (SymbolKind.Property, SymbolKind.Property) => PropertySymbolComparer.Equal((IPropertySymbol)x, (IPropertySymbol)y),
                (SymbolKind.Method, SymbolKind.Method) => MethodSymbolComparer.Equal((IMethodSymbol)x, (IMethodSymbol)y),
                (SymbolKind.Parameter, SymbolKind.Parameter) => ParameterSymbolComparer.Equal((IParameterSymbol)x, (IParameterSymbol)y),
                (SymbolKind.Local, SymbolKind.Local) => LocalSymbolComparer.Equal((ILocalSymbol)x, (ILocalSymbol)y),
                (SymbolKind.NamedType, SymbolKind.NamedType) => NamedTypeSymbolComparer.Equal((INamedTypeSymbol)x, (INamedTypeSymbol)y),
                (SymbolKind.ArrayType, SymbolKind.ArrayType) => TypeSymbolComparer.Equal((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.DynamicType, SymbolKind.DynamicType) => TypeSymbolComparer.Equal((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.ErrorType, SymbolKind.ErrorType) => false,
                (SymbolKind.PointerType, SymbolKind.PointerType) => TypeSymbolComparer.Equal((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.TypeParameter, SymbolKind.TypeParameter) => TypeSymbolComparer.Equal((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.Namespace, SymbolKind.Namespace) => NamespaceSymbolComparer.Equal((INamespaceSymbol)x, (INamespaceSymbol)y),
                (_, _) => SymbolEqualityComparer.Default.Equals(x, y),
            };
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

            return (x.Kind, y.Kind) switch
            {
                (SymbolKind.Field, SymbolKind.Field) => FieldSymbolComparer.Equivalent((IFieldSymbol)x, (IFieldSymbol)y),
                (SymbolKind.Event, SymbolKind.Event) => EventSymbolComparer.Equivalent((IEventSymbol)x, (IEventSymbol)y),
                (SymbolKind.Property, SymbolKind.Property) => PropertySymbolComparer.Equivalent((IPropertySymbol)x, (IPropertySymbol)y),
                (SymbolKind.Method, SymbolKind.Method) => MethodSymbolComparer.Equivalent((IMethodSymbol)x, (IMethodSymbol)y),
                (SymbolKind.Parameter, SymbolKind.Parameter) => ParameterSymbolComparer.Equivalent((IParameterSymbol)x, (IParameterSymbol)y),
                (SymbolKind.Local, SymbolKind.Local) => LocalSymbolComparer.Equal((ILocalSymbol)x, (ILocalSymbol)y),
                (SymbolKind.NamedType, SymbolKind.NamedType) => NamedTypeSymbolComparer.Equal((INamedTypeSymbol)x, (INamedTypeSymbol)y),
                (SymbolKind.ArrayType, SymbolKind.ArrayType) => TypeSymbolComparer.Equivalent((ITypeSymbol)x,     (ITypeSymbol)y),
                (SymbolKind.DynamicType, SymbolKind.DynamicType) => TypeSymbolComparer.Equivalent((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.ErrorType, SymbolKind.ErrorType) => false,
                (SymbolKind.PointerType, SymbolKind.PointerType) => TypeSymbolComparer.Equivalent((ITypeSymbol)x,     (ITypeSymbol)y),
                (SymbolKind.TypeParameter, SymbolKind.TypeParameter) => TypeSymbolComparer.Equivalent((ITypeSymbol)x, (ITypeSymbol)y),
                (SymbolKind.Namespace, SymbolKind.Namespace) => NamespaceSymbolComparer.Equal((INamespaceSymbol)x, (INamespaceSymbol)y),
                (_, _) => SymbolEqualityComparer.Default.Equals(x, y),
            };
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
#pragma warning disable RS1024 // Compare symbols correctly
                return TypeSymbolComparer.GetHashCode(typeSymbol);
#pragma warning restore RS1024 // Compare symbols correctly
            }

            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}

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
                y is null)
            {
                return false;
            }

            if (x is IEventSymbol xEvent &&
                y is IEventSymbol yEvent)
            {
                return EventSymbolComparer.Equal(xEvent, yEvent);
            }

            if (x is IFieldSymbol xField &&
                y is IFieldSymbol yField)
            {
                return FieldSymbolComparer.Equal(xField, yField);
            }

            if (x is ILocalSymbol xLocal &&
                y is ILocalSymbol yLocal)
            {
                return LocalSymbolComparer.Equal(xLocal, yLocal);
            }

            if (x is IMethodSymbol xMethod &&
                y is IMethodSymbol yMethod)
            {
                return MethodSymbolComparer.Equal(xMethod, yMethod);
            }

            if (x is INamedTypeSymbol xNamedType &&
                y is INamedTypeSymbol yNamedType)
            {
                return NamedTypeSymbolComparer.Equal(xNamedType, yNamedType);
            }

            if (x is INamespaceSymbol xNamespace &&
                y is INamespaceSymbol yNamespace)
            {
                return NamespaceSymbolComparer.Equal(xNamespace, yNamespace);
            }

            if (x is IParameterSymbol xParameter &&
                y is IParameterSymbol yParameter)
            {
                return ParameterSymbolComparer.Equal(xParameter, yParameter);
            }

            if (x is IPropertySymbol xProperty &&
                y is IPropertySymbol yProperty)
            {
                return PropertySymbolComparer.Equal(xProperty, yProperty);
            }

            if (x is ITypeSymbol xType &&
                y is ITypeSymbol yType)
            {
                return TypeSymbolComparer.Equal(xType, yType);
            }

            return SymbolEqualityComparer.Default.Equals(x, y);
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
                y is null)
            {
                return false;
            }

            if (x is IEventSymbol xEvent &&
                y is IEventSymbol yEvent)
            {
                return EventSymbolComparer.Equivalent(xEvent, yEvent);
            }

            if (x is IFieldSymbol xField &&
                y is IFieldSymbol yField)
            {
                return FieldSymbolComparer.Equivalent(xField, yField);
            }

            if (x is ILocalSymbol xLocal &&
                y is ILocalSymbol yLocal)
            {
                return LocalSymbolComparer.Equal(xLocal, yLocal);
            }

            if (x is IMethodSymbol xMethod &&
                y is IMethodSymbol yMethod)
            {
                return MethodSymbolComparer.Equivalent(xMethod, yMethod);
            }

            if (x is INamedTypeSymbol xNamedType &&
                y is INamedTypeSymbol yNamedType)
            {
                return NamedTypeSymbolComparer.Equivalent(xNamedType, yNamedType);
            }

            if (x is INamespaceSymbol xNamespace &&
                y is INamespaceSymbol yNamespace)
            {
                return NamespaceSymbolComparer.Equal(xNamespace, yNamespace);
            }

            if (x is IParameterSymbol xParameter &&
                y is IParameterSymbol yParameter)
            {
                return ParameterSymbolComparer.Equivalent(xParameter, yParameter);
            }

            if (x is IPropertySymbol xProperty &&
                y is IPropertySymbol yProperty)
            {
                return PropertySymbolComparer.Equivalent(xProperty, yProperty);
            }

            if (x is ITypeSymbol xType &&
                y is ITypeSymbol yType)
            {
                return TypeSymbolComparer.Equivalent(xType, yType);
            }

            return SymbolEqualityComparer.Default.Equals(x, y);
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

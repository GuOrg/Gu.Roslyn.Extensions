namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

/// <inheritdoc />
public sealed class FieldSymbolComparer : IEqualityComparer<IFieldSymbol>
{
    /// <summary> The default instance. </summary>
    public static readonly FieldSymbolComparer Default = new();

    private FieldSymbolComparer()
    {
    }

    /// <summary> Compares equality by name and containing type. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equal(IFieldSymbol? x, IFieldSymbol? y)
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
               NamedTypeSymbolComparer.Equal(x.ContainingType, y.ContainingType);
    }

    /// <summary> Compares equality by name and containing type and treats overridden and definition as equal. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equivalent(IFieldSymbol? x, IFieldSymbol? y)
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
               Definition(x, y) ||
               Definition(y, x);

        static bool Definition(IFieldSymbol x, IFieldSymbol y)
        {
            return x.IsDefinition &&
                   y is { IsDefinition: false, OriginalDefinition: { } yOriginalDefinition } &&
                   !ReferenceEquals(y, yOriginalDefinition) &&
                   Equivalent(x, yOriginalDefinition);
        }
    }

    /// <summary> Compares equality by name and containing type. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    [Obsolete("Use Equal as RS1024 does not nag about it.")]
    public static bool Equals(IFieldSymbol? x, IFieldSymbol? y) => Equal(x, y);

    //// ReSharper disable once UnusedMember.Global
    //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
    [Obsolete("Should only be called with arguments of type IFieldSymbol.", error: true)]
    public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
    //// ReSharper restore UnusedParameter.Global

    /// <inheritdoc />
    bool IEqualityComparer<IFieldSymbol>.Equals(IFieldSymbol? x, IFieldSymbol? y) => Equal(x, y);

    /// <inheritdoc />
    public int GetHashCode(IFieldSymbol obj)
    {
        return obj?.MetadataName.GetHashCode() ?? 0;
    }
}

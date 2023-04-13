namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

/// <inheritdoc />
public sealed class NamespaceSymbolComparer : IEqualityComparer<INamespaceSymbol>
{
    /// <summary> The default instance. </summary>
    public static readonly NamespaceSymbolComparer Default = new();

    private NamespaceSymbolComparer()
    {
    }

    /// <summary> Determines equality by name. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equal(INamespaceSymbol? x, INamespaceSymbol? y)
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
               Equal(x.ContainingNamespace, y.ContainingNamespace);
    }

    /// <summary> Determines equality by name. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    [Obsolete("Use Equal as RS1024 does not nag about it.")]
    public static bool Equals(INamespaceSymbol? x, INamespaceSymbol? y) => Equal(x, y);

    /// <summary> Determines equality by name. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The string.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equals(INamespaceSymbol x, string y)
    {
        if (x is null)
        {
            throw new ArgumentNullException(nameof(x));
        }

        if (y is null)
        {
            throw new ArgumentNullException(nameof(y));
        }

        var index = y.Length - 1;
        while (true)
        {
            index = y.LastIndexOf('.', index) - 1;
            if (index < 0)
            {
                return y.EqualsAt(x.MetadataName, 0) &&
                       x.ContainingNamespace.IsGlobalNamespace;
            }

            if (!y.EqualsAt(x.MetadataName, index + 2))
            {
                return false;
            }

            if (x is { ContainingNamespace: { IsGlobalNamespace: false } containingNamespace })
            {
                x = containingNamespace;
            }
            else
            {
                return false;
            }
        }
    }

    //// ReSharper disable once UnusedMember.Global
    //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
    [Obsolete("Should only be called with arguments of type INamespaceSymbol.", error: true)]
    public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600
    //// ReSharper restore UnusedParameter.Global

    /// <inheritdoc />
    bool IEqualityComparer<INamespaceSymbol>.Equals(INamespaceSymbol? x, INamespaceSymbol? y) => Equal(x, y);

    /// <inheritdoc />
    public int GetHashCode(INamespaceSymbol obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return obj.MetadataName.GetHashCode();
    }
}

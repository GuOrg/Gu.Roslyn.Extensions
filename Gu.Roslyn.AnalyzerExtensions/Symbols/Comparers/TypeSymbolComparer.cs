namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

/// <inheritdoc />
public sealed class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
{
    /// <summary> The default instance. </summary>
    public static readonly TypeSymbolComparer Default = new();

    private TypeSymbolComparer()
    {
    }

    /// <summary> Determines equality by name, containing type, arity and namespace. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equal(ITypeSymbol? x, ITypeSymbol? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.MetadataName == y.MetadataName &&
               NamespaceSymbolComparer.Equal(x.ContainingNamespace, y.ContainingNamespace) &&
               NullableAnnotation() &&
               TypeArguments();

        bool NullableAnnotation()
        {
            if (!x.IsReferenceType)
            {
                return true;
            }

            return (x.NullableAnnotation, y.NullableAnnotation) switch
            {
                (Microsoft.CodeAnalysis.NullableAnnotation.None, _) => true,
                (_, Microsoft.CodeAnalysis.NullableAnnotation.None) => true,
                var (xa, ya) => xa == ya,
            };
        }

        bool TypeArguments()
        {
            if (x is INamedTypeSymbol { IsGenericType: true, TypeArguments: { } xa } &&
                y is INamedTypeSymbol { IsGenericType: true, TypeArguments: { } ya })
            {
                if (xa.Length != ya.Length)
                {
                    return false;
                }

                for (var i = 0; i < xa.Length; i++)
                {
                    if (!Equal(xa[i], ya[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    /// <summary> Compares equality by name and containing type and treats overridden and definition as equal. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    public static bool Equivalent(ITypeSymbol? x, ITypeSymbol? y)
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

        static bool Definition(ITypeSymbol x, ITypeSymbol y)
        {
            return x.IsDefinition &&
                   y is { IsDefinition: false, OriginalDefinition: { } yOriginalDefinition } &&
                   !ReferenceEquals(y, yOriginalDefinition) &&
                   Equivalent(x, yOriginalDefinition);
        }
    }

    /// <summary> Determines equality by name and containing symbol. </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The other instance.</param>
    /// <returns>True if the instances are found equal.</returns>
    [Obsolete("Use Equal as RS1024 does not nag about it.")]
    public static bool Equals(ITypeSymbol? x, ITypeSymbol? y) => Equal(x, y);

    //// ReSharper disable once UnusedMember.Global
    //// ReSharper disable UnusedParameter.Global
#pragma warning disable CS1591,CA1707,IDE1006,SA1313,SA1600
    [Obsolete("Should only be called with arguments of type ITypeSymbol.", error: true)]
    public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591,CA1707,IDE1006,SA1313,SA1600 // Naming Styles
    //// ReSharper restore UnusedParameter.Global

    /// <summary>Returns the hash code for this string.</summary>
    /// <param name="obj">The instance.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public static int GetHashCode(ITypeSymbol obj)
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
#pragma warning disable RS1024 // Compare symbols correctly
    int IEqualityComparer<ITypeSymbol>.GetHashCode(ITypeSymbol obj) => GetHashCode(obj);
#pragma warning restore RS1024 // Compare symbols correctly
}

namespace Gu.Roslyn.AnalyzerExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

/// <summary>
/// Helpers for constructors.
/// </summary>
public static class Constructor
{
    /// <summary>
    /// Find the first parameterless constructor with a declaration.
    /// </summary>
    /// <param name="type">The <see cref="INamedTypeSymbol"/>.</param>
    /// <param name="search">Specifies if the search is recursive.</param>
    /// <param name="result">The first parameterless constructor found.</param>
    /// <returns>True if a parameterless constructor was found.</returns>
    public static bool TryFindDefault(INamedTypeSymbol type, Search search, [NotNullWhen(true)] out IMethodSymbol? result)
    {
        result = null;
        while (type is { } &&
               type != QualifiedType.System.Object)
        {
            foreach (var candidate in type.Constructors)
            {
                if (candidate is { Parameters.Length: 0, DeclaringSyntaxReferences.Length: 1 } &&
                   NamedTypeSymbolComparer.Equal(candidate.ContainingType, type))
                {
                    result = candidate;
                    return true;
                }
            }

            if (search == Search.TopLevel)
            {
                return false;
            }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            type = type.BaseType;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }

        return false;
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
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
        public static bool TryFindDefault(INamedTypeSymbol type, Search search, out IMethodSymbol result)
        {
            result = null;
            while (type != null &&
                   type != QualifiedType.System.Object)
            {
                foreach (var candidate in type.Constructors)
                {
                    if (candidate.Parameters.Length == 0 &&
                        candidate.ContainingType == type &&
                        candidate.DeclaringSyntaxReferences.Length == 1)
                    {
                        result = candidate;
                        return true;
                    }
                }

                if (search == Search.TopLevel)
                {
                    return false;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}

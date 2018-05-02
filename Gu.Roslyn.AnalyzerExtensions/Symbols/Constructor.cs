namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for constructors
    /// </summary>
    public static class Constructor
    {
        /// <summary>
        /// Find the first parameterless constructor
        /// </summary>
        /// <param name="type">The <see cref="INamedTypeSymbol"/></param>
        /// <param name="search">Specifiec if the search is recursive</param>
        /// <param name="result">The first parameterless ctor found.</param>
        /// <returns>True if a parameterless ctor was found.</returns>
        public static bool TryGetDefault(INamedTypeSymbol type, Search search, out IMethodSymbol result)
        {
            result = null;
            while (type != null &&
                   type != QualifiedType.System.Object)
            {
                foreach (var candidate in type.Constructors)
                {
                    if (candidate.Parameters.Length == 0)
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

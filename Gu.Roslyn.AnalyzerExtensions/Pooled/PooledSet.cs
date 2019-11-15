namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Helpers for working with <see cref="PooledSet{T}"/>.
    /// </summary>
    public static class PooledSet
    {
        /// <summary>
        /// A collection of comparers for different symbol types.
        /// </summary>
        internal static readonly IReadOnlyList<object> SymbolComparers = new object[]
        {
            AssemblySymbolComparer.Default,
            EventSymbolComparer.Default,
            FieldSymbolComparer.Default,
            LocalSymbolComparer.Default,
            MethodSymbolComparer.Default,
            NamedTypeSymbolComparer.Default,
            ParameterSymbolComparer.Default,
            PropertySymbolComparer.Default,
            SymbolComparer.Default,
            TypeSymbolComparer.Default,
        };

        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <typeparam name="T">The type of items in the set.</typeparam>
        /// <returns>A <see cref="PooledSet{T}"/>.</returns>
        public static PooledSet<T> Borrow<T>() => PooledSet<T>.Borrow();

        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <typeparam name="T">The type of elements in the set.</typeparam>
        /// <param name="set">The <see cref="PooledSet{T}"/>.</param>
        /// <returns>The set with incremented usage or a new set if null was passed.</returns>
        public static PooledSet<T> IncrementUsage<T>(this PooledSet<T>? set)
        {
            return PooledSet<T>.BorrowOrIncrementUsage(set);
        }
    }
}

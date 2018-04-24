namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Helpers for working with <see cref="PooledSet{T}"/>
    /// </summary>
    public static class PooledSet
    {
        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        /// <typeparam name="T">The type of elements in the set.</typeparam>
        /// <param name="set">The <see cref="PooledSet{T}"/></param>
        /// <returns>The set with incremented useage or a new set if null was passed.</returns>
        public static PooledSet<T> IncrementUsage<T>(this PooledSet<T> set)
        {
            return PooledSet<T>.BorrowOrIncrementUsage(set);
        }
    }
}

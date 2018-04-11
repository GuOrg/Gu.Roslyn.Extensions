namespace Gu.Roslyn.AnalyzerExtensions
{
    public static class PooledSet
    {
        /// <summary>
        /// The result from this call is meant to be used in a using.
        /// </summary>
        public static PooledSet<T> IncrementUsage<T>(this PooledSet<T> set)
        {
            return PooledSet<T>.BorrowOrIncrementUsage(set);
        }
    }
}

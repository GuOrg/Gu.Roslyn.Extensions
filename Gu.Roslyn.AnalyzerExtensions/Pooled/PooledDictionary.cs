namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Factory methods for <see cref="PooledDictionary{TKey,TValue}"/>.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public static class PooledDictionary
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>
        /// Borrow a dictionary, dispose returns it.
        /// </summary>
        /// <typeparam name="TKey">The type of keys.</typeparam>
        /// <typeparam name="TValue">The type of values.</typeparam>
        /// <returns>A <see cref="PooledDictionary{TKey,TValue}"/>.</returns>
        public static PooledDictionary<TKey, TValue> Borrow<TKey, TValue>()
            where TKey : notnull
        {
            return PooledDictionary<TKey, TValue>.Borrow();
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Factory methods for <see cref="PooledDictionary{TKey,TValue}"/>
    /// </summary>
    public static class PooledDictionary
    {
        /// <summary>
        /// Borrow a dictionary, dispose returns it.
        /// </summary>
        /// <typeparam name="TKey">The type of keys.</typeparam>
        /// <typeparam name="TValue">The type of values.</typeparam>
        /// <returns>A <see cref="PooledDictionary{TKey,TValue}"/></returns>
        public static PooledDictionary<TKey, TValue> Borrow<TKey, TValue>() => PooledDictionary<TKey, TValue>.Borrow();
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helper for using the cache.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Get an item from cache or create and add and return.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="valueFactory">The factory for new items.</param>
        /// <returns>The cached value.</returns>
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory) => Cache<TKey, TValue>.GetOrAdd(key, valueFactory);

        /// <summary>
        /// Controls if Semantic models should be cached for syntax trees.
        /// This can speed up analysis significantly but means Visual Studio uses more memory during compilation.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="context">The <see cref="AnalysisContext"/></param>
        public static void CacheToCompilationEnd<TKey, TValue>(this AnalysisContext context)
        {
#pragma warning disable RS1013 // Start action has no registered non-end actions.
            context.RegisterCompilationStartAction(x =>
#pragma warning restore RS1013 // Start action has no registered non-end actions.
            {
                Cache<TKey, TValue>.Begin();
                x.RegisterCompilationEndAction(_ => Cache<TKey, TValue>.End());
            });
        }
    }
}

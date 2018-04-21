namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;

    public static class Cache
    {
        public static TValue GetOrAdd<TKey, TValue>(TKey key, Func<TKey, TValue> valueFactory) => Cache<TKey, TValue>.GetOrAdd(key, valueFactory);
    }
}

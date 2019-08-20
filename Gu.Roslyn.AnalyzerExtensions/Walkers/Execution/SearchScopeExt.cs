namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Extension methods for <see cref="SearchScope"/>.
    /// </summary>
    public static class SearchScopeExt
    {
        /// <summary>
        /// Check if <paramref name="scope"/> is either <paramref name="scope1"/> or <paramref name="scope2"/>.
        /// </summary>
        /// <param name="scope">The <see cref="SearchScope"/>.</param>
        /// <param name="scope1">The first to match.</param>
        /// <param name="scope2">The other to match.</param>
        /// <returns>True if <paramref name="scope"/> is either <paramref name="scope1"/> or <paramref name="scope2"/>.</returns>
        public static bool IsEither(this SearchScope scope, SearchScope scope1, SearchScope scope2) => scope == scope1 || scope == scope2;
    }
}

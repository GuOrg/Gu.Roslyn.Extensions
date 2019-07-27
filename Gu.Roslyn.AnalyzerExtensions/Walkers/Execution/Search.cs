namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Controls how recursion is handled when walking.
    /// </summary>
    public enum Search
    {
        /// <summary>
        /// Search current scope only
        /// </summary>
        TopLevel,

        /// <summary>
        /// Follow method calls etc, this means the search is more expensive.
        /// </summary>
        Recursive,
    }
}

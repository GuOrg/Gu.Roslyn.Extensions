namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Controls how recursion is handled when walking.
    /// </summary>
    public enum Recursive
    {
        /// <summary>
        /// Search current scope only
        /// </summary>
        No,

        /// <summary>
        /// Follow method calls etc, this means the search is more expensive.
        /// </summary>
        Yes,
    }
}

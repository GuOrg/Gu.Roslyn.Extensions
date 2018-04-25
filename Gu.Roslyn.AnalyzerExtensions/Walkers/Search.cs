namespace Gu.Roslyn.AnalyzerExtensions
{
    public enum Search
    {
        /// <summary>
        /// Search current scope only
        /// </summary>
        TopLevel,

        /// <summary>
        /// Follow method calls etc, this means the search is more expensive.
        /// </summary>
        Recursive
    }
}

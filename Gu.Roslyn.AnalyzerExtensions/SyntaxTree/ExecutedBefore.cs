namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// Result from StatementSyntaxExt.IsExecutedBefore.
    /// </summary>
    public enum ExecutedBefore
    {
        /// <summary>
        /// The analysis failed to analyze the code.
        /// </summary>
        Unknown,

        /// <summary>
        /// Yes, analysis was certain.
        /// </summary>
        Yes,

        /// <summary>
        /// No, analysis was certain.
        /// </summary>
        No,

        /// <summary>
        /// For example when in a loop or in an event handler lambda or when there are goto.
        /// </summary>
        Maybe,
    }
}

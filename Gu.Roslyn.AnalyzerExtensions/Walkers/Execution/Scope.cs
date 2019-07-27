namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// The scope <see cref="ExecutionWalker{T}"/> walks.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// Walks the current node, does not follow invocations.
        /// </summary>
        Member,

        /// <summary>
        /// Walks the current node, follows this. and base. calls.
        /// </summary>
        Instance,

        /// <summary>
        /// Walks the current type, follows this. and base. and static calls.
        /// </summary>
        Type,

        /// <summary>
        /// Follows all calls recursively.
        /// </summary>
        Recursive,
    }
}

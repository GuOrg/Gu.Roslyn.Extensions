namespace Gu.Roslyn.AnalyzerExtensions;

/// <summary>
/// Extension methods for <see cref="ExecutedBefore"/>.
/// </summary>
public static class ExecutedBeforeExt
{
    /// <summary>
    /// Check if <paramref name="candidate"/> is either of <paramref name="x"/> or <paramref name="y"/>.
    /// </summary>
    /// <param name="candidate">The <see cref="ExecutedBefore"/>.</param>
    /// <param name="x">The first kind.</param>
    /// <param name="y">The other kind.</param>
    /// <returns>True if <paramref name="candidate"/> is either of <paramref name="x"/> or <paramref name="y"/>. </returns>
    public static bool IsEither(this ExecutedBefore candidate, ExecutedBefore x, ExecutedBefore y) => candidate == x || candidate == y;
}

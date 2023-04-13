namespace Gu.Roslyn.CodeFixExtensions;

/// <summary>
/// Result type for <see cref="CodeStyle"/>.
/// </summary>
public enum CodeStyleResult
{
    /// <summary>
    /// Did not find the style to use.
    /// </summary>
    NotFound,

    /// <summary>
    /// Found unambiguous yes.
    /// </summary>
    Yes,

    /// <summary>
    /// Found unambiguous no.
    /// </summary>
    No,

    /// <summary>
    /// Found both yes and no.
    /// </summary>
    Mixed,
}

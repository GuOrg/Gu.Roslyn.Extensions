namespace Gu.Roslyn.AnalyzerExtensions;

/// <summary>
/// Factory methods for <see cref="PooledList{T}"/>.
/// </summary>
public static class PooledList
{
    /// <summary>
    /// Borrow a list, dispose returns it.
    /// </summary>
    /// <typeparam name="T">The type of keys.</typeparam>
    /// <returns>A <see cref="PooledList{T}"/>.</returns>
    public static PooledList<T> Borrow<T>() => PooledList<T>.Borrow();
}

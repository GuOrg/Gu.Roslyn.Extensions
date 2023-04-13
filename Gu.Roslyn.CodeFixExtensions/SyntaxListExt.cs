namespace Gu.Roslyn.CodeFixExtensions;

using System;
using Microsoft.CodeAnalysis;

/// <summary>
/// Extension methods for <see cref="SyntaxList{TNode}"/>.
/// </summary>
public static class SyntaxListExt
{
    /// <summary>
    /// Move the item at oldIndex to newIndex.
    /// </summary>
    /// <typeparam name="T">The type of items in <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="SyntaxList{T}"/>.</param>
    /// <param name="oldIndex">The old index.</param>
    /// <param name="newIndex">The new index.</param>
    /// <returns>A <see cref="SyntaxList{T}"/> with the item moved.</returns>
    public static SyntaxList<T> Move<T>(this SyntaxList<T> list, int oldIndex, int newIndex)
        where T : SyntaxNode
    {
        var item = list[oldIndex];
        return list.RemoveAt(oldIndex)
                   .Insert(Math.Min(newIndex, list.Count - 1), item);
    }
}

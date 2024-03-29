﻿#pragma warning disable RS0041 // Public members should not use oblivious types
#nullable enable

namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Extension methods that avoids allocations.
/// </summary>
public static partial class EnumerableExt
{
    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt<T>(this IReadOnlyList<T> source, int index, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                for (int j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult)
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this IReadOnlyList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult temp &&
                        predicate(temp))
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this IReadOnlyList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this IReadOnlyList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this IReadOnlyList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        if (source.Count == 0)
        {
            result = default(T);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this IReadOnlyList<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this IReadOnlyList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this IReadOnlyList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(T);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt<T>(this ImmutableArray<T> source, int index, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (index < 0 ||
            source.Length <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (source.Length == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] is TResult item)
            {
                for (int j = i + 1; j < source.Length; j++)
                {
                    if (source[j] is TResult)
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this ImmutableArray<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                for (var j = i + 1; j < source.Length; j++)
                {
                    if (source[j] is TResult temp &&
                        predicate(temp))
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this ImmutableArray<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        for (var i = 0; i < source.Length; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Length; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (source.Length == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this ImmutableArray<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this ImmutableArray<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (source.Length == 0)
        {
            result = default(T);
            return false;
        }

        result = source[source.Length - 1];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this ImmutableArray<T> source, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = source.Length - 1; i >= 0; i--)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this ImmutableArray<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where TResult : T
    {
        result = default;
        for (var i = source.Length - 1; i >= 0; i--)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this ImmutableArray<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
    {
        result = default;
        for (var i = source.Length - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(T);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt(this ChildSyntaxList source, int index, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this ChildSyntaxList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this ChildSyntaxList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this ChildSyntaxList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this ChildSyntaxList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this ChildSyntaxList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(SyntaxNodeOrToken);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this ChildSyntaxList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(SyntaxNodeOrToken);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt<T>(this SeparatedSyntaxList<T> source, int index, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                for (int j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult)
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this SeparatedSyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult temp &&
                        predicate(temp))
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this SeparatedSyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(T);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this SeparatedSyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this SeparatedSyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(T);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt<T>(this SyntaxList<T> source, int index, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this SyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this SyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                for (int j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult)
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingleOfType<T, TResult>(this SyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (source[j] is TResult temp &&
                        predicate(temp))
                    {
                        return false;
                    }
                }

                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle<T>(this SyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this SyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this SyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirstOfType<T, TResult>(this SyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst<T>(this SyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this SyntaxList<T> source, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(T);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this SyntaxList<T> source, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item)
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type to filter by.</typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The filter</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLastOfType<T, TResult>(this SyntaxList<T> source, Func<TResult, bool> predicate, [MaybeNullWhen(false)] out TResult result)
        where T : SyntaxNode
        where TResult : T
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            if (source[i] is TResult item &&
                predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast<T>(this SyntaxList<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T result)
        where T : SyntaxNode
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(T);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt(this SyntaxNodeOrTokenList source, int index, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxNodeOrTokenList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxNodeOrTokenList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxNodeOrTokenList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxNodeOrTokenList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxNodeOrTokenList source, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(SyntaxNodeOrToken);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxNodeOrTokenList source, Func<SyntaxNodeOrToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxNodeOrToken result)
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(SyntaxNodeOrToken);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt(this SyntaxTokenList source, int index, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxTokenList source, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxTokenList source, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxTokenList source, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(SyntaxToken);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate, [MaybeNullWhen(false)] out SyntaxToken result)
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(SyntaxToken);
        return false;
    }

    /// <summary>
    /// Try getting the element at <paramref name="index"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="index">The index.</param>
    /// <param name="result">The element at index if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryElementAt(this SyntaxTriviaList source, int index, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        if (index < 0 ||
            source.Count <= index)
        {
            return false;
        }

        result = source[index];
        return true;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The single element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxTriviaList source, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        if (source.Count == 1)
        {
            result = source[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The single element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TrySingle(this SyntaxTriviaList source, Func<SyntaxTrivia, bool> predicate, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (predicate(source[j]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The first element, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxTriviaList source, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        if (source.Count == 0)
        {
            return false;
        }

        result = source[0];
        return true;
    }

    /// <summary>
    /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The first element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryFirst(this SyntaxTriviaList source, Func<SyntaxTrivia, bool> predicate, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="result">The last element if found, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxTriviaList source, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        if (source.Count == 0)
        {
            result = default(SyntaxTrivia);
            return false;
        }

        result = source[source.Count - 1];
        return true;
    }

    /// <summary>
    /// Try getting the last element in <paramref name="source"/> matching <paramref name="predicate"/>
    /// </summary>
    /// <param name="source">The source collection, can be null.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="result">The last element matching the predicate, can be null.</param>
    /// <returns>True if an element was found.</returns>
    public static bool TryLast(this SyntaxTriviaList source, Func<SyntaxTrivia, bool> predicate, [MaybeNullWhen(false)] out SyntaxTrivia result)
    {
        result = default;
        for (var i = source.Count - 1; i >= 0; i--)
        {
            var item = source[i];
            if (predicate(item))
            {
                result = item;
                return true;
            }
        }

        result = default(SyntaxTrivia);
        return false;
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;

    /// <summary>
    /// Helpers for working with strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Check if <paramref name="text"/> is start + end
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="start">The start</param>
        /// <param name="end">The end</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/></param>
        /// <returns>True if <paramref name="text"/> is start + end</returns>
        public static bool IsParts(this string text, string start, string end, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (text == null)
            {
                return start == null && end == null;
            }

            if (string.IsNullOrEmpty(start))
            {
                return string.Equals(text, end, stringComparison);
            }

            if (string.IsNullOrEmpty(end))
            {
                return string.Equals(text, start, stringComparison);
            }

            if (text.Length != start.Length + end.Length)
            {
                return false;
            }

            return text.StartsWith(start, stringComparison) &&
                   text.EndsWith(end, stringComparison);
        }

        /// <summary>
        /// Check if <paramref name="text"/> is start + middle + end
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="start">The start</param>
        /// <param name="middle">The middle</param>
        /// <param name="end">The end</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/></param>
        /// <returns>True if <paramref name="text"/> is start + end</returns>
        public static bool IsParts(this string text, string start, string middle, string end, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (text == null)
            {
                return start == null && middle == null && end == null;
            }

            if (string.IsNullOrEmpty(start))
            {
                return text.IsParts(middle, end, stringComparison);
            }

            if (string.IsNullOrEmpty(middle))
            {
                return text.IsParts(start, end, stringComparison);
            }

            if (string.IsNullOrEmpty(end))
            {
                return text.IsParts(start, middle, stringComparison);
            }

            if (text.Length != start.Length + middle.Length + end.Length)
            {
                return false;
            }

            return text.StartsWith(start, stringComparison) &&
                   text.IndexOf(middle, start.Length, stringComparison) == start.Length &&
                   text.EndsWith(end, stringComparison);
        }

        /// <summary>
        /// Check if <paramref name="text"/> is start + middle + end
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="part1">Part 1</param>
        /// <param name="part2">Part 2</param>
        /// <param name="part3">Part 3</param>
        /// <param name="part4">Part 4</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/></param>
        /// <returns>True if <paramref name="text"/> is start + end</returns>
        public static bool IsParts(this string text, string part1, string part2, string part3, string part4, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (text == null)
            {
                return part1 == null && part2 == null && part3 == null && part4 == null;
            }

            if (string.IsNullOrEmpty(part1))
            {
                return text.IsParts(part2, part3, part4, stringComparison);
            }

            if (string.IsNullOrEmpty(part2))
            {
                return text.IsParts(part1, part3, part4, stringComparison);
            }

            if (string.IsNullOrEmpty(part3))
            {
                return text.IsParts(part1, part2, part4, stringComparison);
            }

            if (string.IsNullOrEmpty(part4))
            {
                return text.IsParts(part1, part2, part3, stringComparison);
            }

            if (text.Length != part1.Length + part2.Length + part3.Length + part4.Length)
            {
                return false;
            }

            return text.StartsWith(part1, stringComparison) &&
                   text.IndexOf(part2, part1.Length, stringComparison) == part1.Length &&
                   text.IndexOf(part3, part1.Length + part2.Length, stringComparison) == part1.Length + part2.Length &&
                   text.EndsWith(part4, stringComparison);
        }
    }
}

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

            if (start == null)
            {
                return string.Equals(text, end, stringComparison);
            }

            if (end == null)
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

            if (start == null)
            {
                return text.IsParts(middle, end, stringComparison);
            }

            if (middle == null)
            {
                return text.IsParts(start, end, stringComparison);
            }

            if (end == null)
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
    }
}

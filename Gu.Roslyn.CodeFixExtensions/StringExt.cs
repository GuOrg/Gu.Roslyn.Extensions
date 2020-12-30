namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Diagnostics;
    using Gu.Roslyn.AnalyzerExtensions;

    /// <summary>
    /// Helpers for working with strings.
    /// </summary>
    public static class StringExt
    {
        /// <summary>Adjust each row in <paramref name="text"/> to start with <paramref name="leadingWhitespace"/>.</summary>
        /// <param name="text">The text.</param>
        /// <param name="leadingWhitespace">The whitespace to adjust each row to have.</param>
        /// <returns><paramref name="text"/> with each line adjusted to start with <paramref name="leadingWhitespace"/>.</returns>
        public static string WithLeadingWhiteSpace(this string text, string? leadingWhitespace)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (leadingWhitespace is null)
            {
                return text;
            }

            var eol = text.IndexOf('\n');
            if (eol < 0 ||
                eol == text.Length - 1)
            {
                var substring = FindSubstring(0, text.Length);
                return leadingWhitespace + text.Substring(substring.Start, substring.Length);
            }

            var builder = StringBuilderPool.Borrow();
            var pos = 0;
            do
            {
                var substring = FindSubstring(pos, eol + 1);
                _ = builder.Append(leadingWhitespace)
                       .Append(text, substring.Start, substring.Length);
                pos = eol + 1;
            }
            while ((eol = text.IndexOf('\n', pos)) > 0);

            if (pos < text.Length - 1)
            {
                var substring = FindSubstring(pos, text.Length);
                _ = builder.Append(leadingWhitespace)
                           .Append(text, substring.Start, substring.Length);
            }

            return builder.Return();

            Substring FindSubstring(int start, int end)
            {
                if (text[start] != ' ')
                {
                    return new Substring(start, end);
                }

                if (string.IsNullOrEmpty(leadingWhitespace))
                {
                    return new Substring(text.CountWhile(x => x == ' ', start, end), end);
                }

                var indexOf = text.IndexOf(leadingWhitespace, StringComparison.Ordinal);
                if (indexOf == 0)
                {
                    var offset = start;
                    while (text.StartsWith(offset + 1, leadingWhitespace!))
                    {
                        offset++;
                    }

                    return new Substring(offset + leadingWhitespace!.Length, end);
                }

                if (indexOf > 0)
                {
                    return IsWhitespaceTo(text, start, indexOf)
                        ? new Substring(indexOf, end)
                        : new Substring(start, end);
                }

                return new Substring(text.CountWhile(x => x == ' ', start, end), end);
            }
        }

        /// <summary>
        /// Get <paramref name="text"/> with first char lowercase.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><paramref name="text"/>  with first char lower.</returns>
        public static string ToFirstCharLower(this string text)
        {
            if (string.IsNullOrEmpty(text) ||
                char.IsLower(text[0]))
            {
                return text;
            }

            var chars = text.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);
            return new string(chars);
        }

        /// <summary>
        /// Get <paramref name="text"/> with first char uppercase.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><paramref name="text"/>  with first char lower.</returns>
        public static string ToFirstCharUpper(this string text)
        {
            if (string.IsNullOrEmpty(text) ||
                char.IsUpper(text[0]))
            {
                return text;
            }

            var chars = text.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        private static bool IsWhitespaceTo(this string line, int start, int position)
        {
            for (var i = start; i < position; i++)
            {
                if (line[i] != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool StartsWith(this string line, int position, string leadingWhitespace)
        {
            if (line.Length > position + leadingWhitespace.Length)
            {
                for (var i = 0; i < leadingWhitespace.Length; i++)
                {
                    if (line[position + i] != leadingWhitespace[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private static int CountWhile(this string text, Func<char, bool> predicate, int start, int end)
        {
            var count = 0;
            for (var i = start; i < end; i++)
            {
                var c = text[i];
                if (predicate(c))
                {
                    count++;
                }
            }

            return count;
        }

        [DebuggerDisplay("Start: {Start}, End: {End}, Length: {Length}")]
        private readonly struct Substring
        {
            internal Substring(int start, int end)
            {
                this.Start = start;
                this.End = end;
            }

            internal int Start { get; }

            internal int End { get; }

            internal int Length => this.End - this.Start;
        }
    }
}

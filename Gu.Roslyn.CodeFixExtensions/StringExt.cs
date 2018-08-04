namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Gu.Roslyn.AnalyzerExtensions;

    /// <summary>
    /// Helpers for working with strings
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Prepend each row in <paramref name="text"/> with <paramref name="leadingWhitespace"/>
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="leadingWhitespace">The whitespace to prepend</param>
        /// <returns></returns>
        public static string WithLeadingWhiteSpace(this string text, string leadingWhitespace)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (leadingWhitespace == null)
            {
                return text;
            }

            var indexOf = text.IndexOf('\n');
            if (indexOf < 0)
            {
                return AdjustWhitespace(text, leadingWhitespace);
            }

            var builder = StringBuilderPool.Borrow();
            var start = 0;
            foreach (var line in text.Split('\n'))
            {
                builder.Append($"{AdjustWhitespace(line, leadingWhitespace)}\n");
            }

            return builder.Return();
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

        private static string AdjustWhitespace(string line, string leadingWhitespace)
        {
            if (line[0] != ' ')
            {
                return leadingWhitespace + line;
            }

            var indexOf = line.IndexOf(leadingWhitespace, StringComparison.Ordinal);
            if (indexOf == 0)
            {
                var offset = 0;
                while (line.StartsWith(offset + 1, leadingWhitespace))
                {
                    offset++;
                }

                return line.Substring(offset);
            }

            if (indexOf > 0)
            {
                return IsWhitespaceTo(line, indexOf)
                    ? line.Substring(indexOf)
                    : leadingWhitespace + line;
            }

            return leadingWhitespace + line.TrimStart(' ');
        }

        private static bool IsWhitespaceTo(this string line, int position)
        {
            for (int i = 0; i < position; i++)
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
    }
}

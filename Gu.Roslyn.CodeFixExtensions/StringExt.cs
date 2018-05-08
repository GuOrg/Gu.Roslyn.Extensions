namespace Gu.Roslyn.CodeFixExtensions
{
    /// <summary>
    /// Helpers for working with strings
    /// </summary>
    public static class StringExt
    {
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
    }
}

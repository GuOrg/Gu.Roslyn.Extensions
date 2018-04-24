namespace Gu.Roslyn.CodeFixExtensions
{
    /// <summary>
    /// Helpers for working with strings
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Get <paramref name="text"/>  with first char lower.
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
    }
}

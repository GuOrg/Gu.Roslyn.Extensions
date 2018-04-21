namespace Gu.Roslyn.CodeFixExtensions
{
    public static class StringExt
    {
        public static string ToFirstCharLower(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var chars = text.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);
            return new string(chars);
        }
    }
}

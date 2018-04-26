namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for <see cref="Accessibility"/>
    /// </summary>
    public static class AccessibilityExt
    {
        /// <summary>
        /// Check if <paramref name="accessibility"/> is either of <paramref name="x"/> or <paramref name="y"/>
        /// </summary>
        /// <param name="accessibility">The <see cref="Accessibility"/></param>
        /// <param name="x">The first kind</param>
        /// <param name="y">The other kind</param>
        /// <returns>True if <paramref name="accessibility"/> is either of <paramref name="x"/> or <paramref name="y"/> </returns>
        public static bool IsEither(this Accessibility accessibility, Accessibility x, Accessibility y) => accessibility == x || accessibility == y;

        /// <summary>
        /// Check if <paramref name="accessibility"/> is either of <paramref name="x"/> or <paramref name="y"/> or <paramref name="z"/>
        /// </summary>
        /// <param name="accessibility">The <see cref="Accessibility"/></param>
        /// <param name="x">The first kind</param>
        /// <param name="y">The other kind</param>
        /// <param name="z">The third kind</param>
        /// <returns>True if <paramref name="accessibility"/> is either of <paramref name="x"/> or <paramref name="y"/> </returns>
        public static bool IsEither(this Accessibility accessibility, Accessibility x, Accessibility y, Accessibility z) => accessibility == x || accessibility == y || accessibility == z;

        /// <summary>
        /// Return the string used in code for representing <paramref name="accessibility"/>
        /// </summary>
        /// <param name="accessibility">The <see cref="Accessibility"/></param>
        /// <returns>The string used in code for representing <paramref name="accessibility"/></returns>
        public static string ToCodeString(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.NotApplicable:
                    break;
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "private protected";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.ProtectedOrInternal:
                    return "internal protected";
                case Accessibility.Public:
                    return "public";
                default:
                    throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
            }

            return string.Empty;
        }
    }
}

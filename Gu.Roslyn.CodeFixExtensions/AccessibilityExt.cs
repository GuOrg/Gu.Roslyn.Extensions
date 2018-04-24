namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for <see cref="Accessibility"/>
    /// </summary>
    public static class AccessibilityExt
    {
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

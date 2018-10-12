namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxTokenList"/>.
    /// </summary>
    public static class SyntaxTokenListExt
    {
        /// <summary>
        /// Check if the list contains any of <paramref name="k1"/> or <paramref name="k2"/>.
        /// </summary>
        /// <param name="list">The <see cref="SyntaxTokenList"/>.</param>
        /// <param name="k1">The first <see cref="SyntaxKind"/>.</param>
        /// <param name="k2">The other <see cref="SyntaxKind"/>.</param>
        /// <returns>True if match was found.</returns>
        public static bool Any(this SyntaxTokenList list, SyntaxKind k1, SyntaxKind k2) => list.Any(k1) || list.Any(k2);

        /// <summary>
        /// Check if the list contains any of <paramref name="k1"/> or <paramref name="k2"/> or <paramref name="k3"/>.
        /// </summary>
        /// <param name="list">The <see cref="SyntaxTokenList"/>.</param>
        /// <param name="k1">The first <see cref="SyntaxKind"/>.</param>
        /// <param name="k2">The other <see cref="SyntaxKind"/>.</param>
        /// <param name="k3">The third <see cref="SyntaxKind"/>.</param>
        /// <returns>True if match was found.</returns>
        public static bool Any(this SyntaxTokenList list, SyntaxKind k1, SyntaxKind k2, SyntaxKind k3) => list.Any(k1) || list.Any(k2) || list.Any(k3);

        /// <summary>
        /// Get the corresponding accessibility.
        /// </summary>
        /// <param name="list">The <see cref="SyntaxTokenList"/> with modifiers.</param>
        /// <param name="whenMissing">The default accessibility.</param>
        /// <returns>The <see cref="Accessibility"/>.</returns>
        public static Accessibility Accessibility(this SyntaxTokenList list, Accessibility whenMissing)
        {
            if (list.Any(SyntaxKind.PublicKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            if (list.Any(SyntaxKind.InternalKeyword))
            {
                if (list.Any(SyntaxKind.ProtectedKeyword))
                {
                    return Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal;
                }

                ////if (list.Any(SyntaxKind.PrivateKeyword))
                ////{
                ////    return Microsoft.CodeAnalysis.Accessibility.PrivateProtected;
                ////}

                return Microsoft.CodeAnalysis.Accessibility.Internal;
            }

            if (list.Any(SyntaxKind.ProtectedKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Protected;
            }

            if (list.Any(SyntaxKind.PrivateKeyword))
            {
                return Microsoft.CodeAnalysis.Accessibility.Private;
            }

            return whenMissing;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class SyntaxTokenListExt
    {
        public static bool Any(this SyntaxTokenList list, SyntaxKind k1, SyntaxKind k2) => list.Any(k1) || list.Any(k2);

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

            return whenMissing;
        }
    }
}

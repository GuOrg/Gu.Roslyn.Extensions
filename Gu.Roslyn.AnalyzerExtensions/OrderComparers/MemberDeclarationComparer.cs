namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class MemberDeclarationComparer
    {
        internal static int CompareScope(SyntaxTokenList x, SyntaxTokenList y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.ConstKeyword))
                {
                    return 0;
                }

                if (list.Any(SyntaxKind.StaticKeyword))
                {
                    return 1;
                }

                return 2;
            }
        }

        internal static int CompareAccessability(SyntaxTokenList x, SyntaxTokenList y, Accessibility @default)
        {
            return CompareAccessability(
                x.Accessibility(@default),
                y.Accessibility(@default));
        }

        internal static int CompareAccessability(Accessibility x, Accessibility y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(Accessibility accessibility)
            {
                switch (accessibility)
                {
                    case Accessibility.Public:
                        return 0;
                    case Accessibility.Internal:
                        return 1;
                    case Accessibility.ProtectedAndInternal:
                        return 2;
                    case Accessibility.Protected:
                        return 3;
                    case Accessibility.Private:
                        return 4;
                    default:
                        return 5;
                }
            }
        }
    }
}

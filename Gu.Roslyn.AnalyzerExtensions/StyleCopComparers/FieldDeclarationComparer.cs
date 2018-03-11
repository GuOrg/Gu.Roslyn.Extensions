namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class FieldDeclarationComparer : IComparer<FieldDeclarationSyntax>
    {
        public static readonly FieldDeclarationComparer Default = new FieldDeclarationComparer();

        public int Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            var compare = CompareAccessability(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareConstStaticInstance(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            return CompareReadOnly(x.Modifiers, y.Modifiers);
        }

        private static int CompareAccessability(SyntaxTokenList x, SyntaxTokenList y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.PublicKeyword))
                {
                    return 0;
                }

                if (list.Any(SyntaxKind.ProtectedKeyword) &&
                    list.Any(SyntaxKind.InternalKeyword))
                {
                    return 1;
                }

                if (list.Any(SyntaxKind.InternalKeyword))
                {
                    return 2;
                }

                if (list.Any(SyntaxKind.ProtectedKeyword))
                {
                    return 3;
                }

                if (list.Any(SyntaxKind.PrivateKeyword))
                {
                    return 4;
                }

                return 5;
            }
        }

        private static int CompareConstStaticInstance(SyntaxTokenList x, SyntaxTokenList y)
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

        private static int CompareReadOnly(SyntaxTokenList x, SyntaxTokenList y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    return 0;
                }

                return 1;
            }
        }
    }
}

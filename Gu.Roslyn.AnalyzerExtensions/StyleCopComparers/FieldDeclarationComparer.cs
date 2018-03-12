namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class FieldDeclarationComparer : IComparer<FieldDeclarationSyntax>
    {
        public static readonly FieldDeclarationComparer Default = new FieldDeclarationComparer();

        public static int Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (IsInitializedWith(x, y))
            {
                return 1;
            }

            if (IsInitializedWith(y, x))
            {
                return -1;
            }

            var compare = CompareAccessability(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareReadOnly(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<FieldDeclarationSyntax>.Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            return Compare(x, y);
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

                return 4;
            }
        }

        private static int CompareScope(SyntaxTokenList x, SyntaxTokenList y)
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

        private static bool IsInitializedWith(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (y.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword) &&
                x.Declaration.Variables.TryLast(out var variable) &&
                variable.Initializer is EqualsValueClauseSyntax initializer &&
                !(initializer.Value is LiteralExpressionSyntax))
            {
                using (var walker = IdentifierNameWalker.Borrow(initializer))
                {
                    foreach (var identifierName in walker.IdentifierNames)
                    {
                        if (y.Declaration.Variables.TryFirst(v => v.Identifier.ValueText == identifierName.Identifier.ValueText, out _))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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

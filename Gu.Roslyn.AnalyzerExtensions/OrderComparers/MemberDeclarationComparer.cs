namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MemberDeclarationComparer
    {
        public static int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            if (TryCompare<FieldDeclarationSyntax>(x, y, FieldDeclarationComparer.Compare, out var result) ||
                TryCompare<ConstructorDeclarationSyntax>(x, y, ConstructorDeclarationComparer.Compare, out result) ||
                TryCompareEvent(x, y, out result) ||
                TryCompare<PropertyDeclarationSyntax>(x, y, PropertyDeclarationComparer.Compare, out result) ||
                TryCompare<IndexerDeclarationSyntax>(x, y, IndexerDeclarationComparer.Compare, out result) ||
                TryCompare<MethodDeclarationSyntax>(x, y, MethodDeclarationComparer.Compare, out result))
            {
                return result;
            }

            return 0;
        }

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

        internal static int CompareSpanStart(int x, int y)
        {
            if (x == 0 || y == 0)
            {
                return 0;
            }

            return x.CompareTo(y);
        }

        private static bool TryCompare<T>(MemberDeclarationSyntax x, MemberDeclarationSyntax y, Func<T, T, int> compare, out int result)
            where T : MemberDeclarationSyntax
        {
            if (x is T xt)
            {
                result = y is T yt ? compare(xt, yt) : -1;
                return true;
            }

            if (y is T)
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool TryCompareEvent(MemberDeclarationSyntax x, MemberDeclarationSyntax y, out int result)
        {
            if (IsEvent(x))
            {
                if (IsEvent(y))
                {
                    result = CompareAccessability(Accessibility(x), Accessibility(y));
                    if (result != 0)
                    {
                        return true;
                    }

                    result = CompareScope(Modifiers(x), Modifiers(y));
                    if (result != 0)
                    {
                        return true;
                    }

                    result = CompareSpanStart(x.SpanStart, y.SpanStart);
                    return true;
                }

                result = -1;
                return true;
            }

            if (IsEvent(y))
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;

            bool IsEvent(MemberDeclarationSyntax candidate) => candidate is EventDeclarationSyntax ||
                                                               candidate is EventFieldDeclarationSyntax;

            Accessibility Accessibility(MemberDeclarationSyntax member)
            {
                switch (member)
                {
                    case EventDeclarationSyntax eventDeclaration:
                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Microsoft.CodeAnalysis.Accessibility.Public;
                        }

                        return eventDeclaration.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
                    case EventFieldDeclarationSyntax eventField:
                        return eventField.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
                    default:
                        return Microsoft.CodeAnalysis.Accessibility.NotApplicable;
                }
            }

            SyntaxTokenList Modifiers(MemberDeclarationSyntax member)
            {
                switch (member)
                {
                    case EventDeclarationSyntax eventDeclaration:
                        return eventDeclaration.Modifiers;
                    case EventFieldDeclarationSyntax eventField:
                        return eventField.Modifiers;
                    default:
                        return default(SyntaxTokenList);
                }
            }
        }
    }
}

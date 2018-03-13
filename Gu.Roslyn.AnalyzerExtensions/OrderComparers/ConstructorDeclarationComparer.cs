namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ConstructorDeclarationComparer : IComparer<ConstructorDeclarationSyntax>
    {
        public static readonly ConstructorDeclarationComparer Default = new ConstructorDeclarationComparer();

        public static int Compare(ConstructorDeclarationSyntax x, ConstructorDeclarationSyntax y)
        {
            if (ReferenceEquals(x, y))
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

            var compare = BaseMethodDeclarationComparer.CompareScope(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareAccessability(x, y);
            if (compare != 0)
            {
                return compare;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<ConstructorDeclarationSyntax>.Compare(ConstructorDeclarationSyntax x, ConstructorDeclarationSyntax y) => Compare(x, y);

        private static int CompareAccessability(BaseMethodDeclarationSyntax x, BaseMethodDeclarationSyntax y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(BaseMethodDeclarationSyntax method)
            {
                if (method.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    return 0;
                }

                if (method.Modifiers.Any(SyntaxKind.ProtectedKeyword) &&
                    method.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return 1;
                }

                if (method.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return 2;
                }

                if (method.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    return 3;
                }

                return 4;
            }
        }
    }
}

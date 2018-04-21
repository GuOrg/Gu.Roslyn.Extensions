namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public sealed class BaseMethodDeclarationComparer : IComparer<BaseMethodDeclarationSyntax>
    {
        public static readonly BaseMethodDeclarationComparer Default = new BaseMethodDeclarationComparer();

        public static int Compare(BaseMethodDeclarationSyntax x, BaseMethodDeclarationSyntax y)
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

            if (x is ConstructorDeclarationSyntax xCtor)
            {
                if (y is ConstructorDeclarationSyntax yCtor)
                {
                    return ConstructorDeclarationComparer.Compare(xCtor, yCtor);
                }

                return -1;
            }

            if (y is ConstructorDeclarationSyntax)
            {
                return 1;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<BaseMethodDeclarationSyntax>.Compare(BaseMethodDeclarationSyntax x, BaseMethodDeclarationSyntax y) => Compare(x, y);

        internal static int CompareScope(BaseMethodDeclarationSyntax x, BaseMethodDeclarationSyntax y)
        {
            return Index(x.Modifiers).CompareTo(Index(y.Modifiers));

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.StaticKeyword))
                {
                    return 0;
                }

                return 1;
            }
        }
    }
}

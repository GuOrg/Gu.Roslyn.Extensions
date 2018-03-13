namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodDeclarationComparer : IComparer<MethodDeclarationSyntax>
    {
        public static readonly MethodDeclarationComparer Default = new MethodDeclarationComparer();

        public static int Compare(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
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

            var compare = CompareAccessability(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = BaseMethodDeclarationComparer.CompareScope(x, y);
            if (compare != 0)
            {
                return compare;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<MethodDeclarationSyntax>.Compare(MethodDeclarationSyntax x, MethodDeclarationSyntax y) => Compare(x, y);

        private static int CompareAccessability(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
        {
            return MemberDeclarationComparer.CompareAccessability(Accessibility(x), Accessibility(y));

            Accessibility Accessibility(MethodDeclarationSyntax method)
            {
                if (method.ExplicitInterfaceSpecifier != null)
                {
                    return Microsoft.CodeAnalysis.Accessibility.Public;
                }

                return method.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
            }
        }
    }
}

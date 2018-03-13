namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
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

            compare = MemberDeclarationComparer.CompareAccessability(x.Modifiers, y.Modifiers, Accessibility.Private);
            if (compare != 0)
            {
                return compare;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<ConstructorDeclarationSyntax>.Compare(ConstructorDeclarationSyntax x, ConstructorDeclarationSyntax y) => Compare(x, y);
    }
}

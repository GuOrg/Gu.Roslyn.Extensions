namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class BasePropertyDeclarationComparer : IComparer<BasePropertyDeclarationSyntax>
    {
        public static readonly BasePropertyDeclarationComparer Default = new BasePropertyDeclarationComparer();

        public static int Compare(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y)
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

            if (x is EventDeclarationSyntax &&
                !(y is EventDeclarationSyntax))
            {
                return -1;
            }

            if (!(x is EventDeclarationSyntax) &&
                y is EventDeclarationSyntax)
            {
                return 1;
            }

            if (x is IndexerDeclarationSyntax &&
                !(y is IndexerDeclarationSyntax))
            {
                return 1;
            }

            if (!(x is IndexerDeclarationSyntax) &&
                y is IndexerDeclarationSyntax)
            {
                return -1;
            }

            if (x is PropertyDeclarationSyntax xProperty &&
                y is PropertyDeclarationSyntax yProperty)
            {
                return PropertyDeclarationComparer.Compare(xProperty, yProperty);
            }

            var compare = MemberDeclarationComparer.CompareAccessability(x.Modifiers, y.Modifiers, Accessibility.Private);
            if (compare != 0)
            {
                return compare;
            }

            compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            if (x.TryGetGetter(out _) &&
                !y.TryGetGetter(out _))
            {
                return -1;
            }

            if (!x.TryGetGetter(out _) &&
                y.TryGetGetter(out _))
            {
                return 1;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<BasePropertyDeclarationSyntax>.Compare(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y) => Compare(x, y);
    }
}

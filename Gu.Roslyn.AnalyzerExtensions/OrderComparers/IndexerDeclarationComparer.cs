namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class IndexerDeclarationComparer : IComparer<IndexerDeclarationSyntax>
    {
        public static readonly IndexerDeclarationComparer Default = new IndexerDeclarationComparer();

        public static int Compare(IndexerDeclarationSyntax x, IndexerDeclarationSyntax y)
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

            var compare = MemberDeclarationComparer.CompareAccessability(Accessibility(x), Accessibility(y));
            if (compare != 0)
            {
                return compare;
            }

            compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            return MemberDeclarationComparer.CompareSpanStart(x.SpanStart, y.SpanStart);
        }

        int IComparer<IndexerDeclarationSyntax>.Compare(IndexerDeclarationSyntax x, IndexerDeclarationSyntax y) => Compare(x, y);

        private static Accessibility Accessibility(IndexerDeclarationSyntax indexer)
        {
            if (indexer.ExplicitInterfaceSpecifier != null)
            {
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            return indexer.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
        }
    }
}

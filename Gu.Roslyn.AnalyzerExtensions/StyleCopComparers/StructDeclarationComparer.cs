namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class StructDeclarationComparer : IComparer<StructDeclarationSyntax>
    {
        /// <summary> The default instance. </summary>
        public static readonly StructDeclarationComparer Default = new StructDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
        public static int Compare(StructDeclarationSyntax x, StructDeclarationSyntax y)
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

            var compare = MemberDeclarationComparer.CompareAccessibility(Accessibility(x), Accessibility(y));
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

        /// <inheritdoc />
        int IComparer<StructDeclarationSyntax>.Compare(StructDeclarationSyntax x, StructDeclarationSyntax y) => Compare(x, y);

        private static Accessibility Accessibility(StructDeclarationSyntax method)
        {
            return method.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
        }
    }
}
namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class MethodDeclarationComparer : IComparer<MethodDeclarationSyntax>
    {
        /// <summary> The default instance. </summary>
        public static readonly MethodDeclarationComparer Default = new MethodDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
        public static int Compare(MethodDeclarationSyntax? x, MethodDeclarationSyntax? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
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

            return MemberDeclarationComparer.CompareSpanStart(x, y);
        }

        /// <inheritdoc />
        int IComparer<MethodDeclarationSyntax>.Compare(MethodDeclarationSyntax? x, MethodDeclarationSyntax? y) => Compare(x, y);

        private static Accessibility Accessibility(MethodDeclarationSyntax declaration)
        {
            if (declaration.ExplicitInterfaceSpecifier != null)
            {
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            return declaration.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
        }
    }
}

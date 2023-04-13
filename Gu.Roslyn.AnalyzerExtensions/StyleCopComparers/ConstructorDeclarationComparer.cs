namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <inheritdoc />
public sealed class ConstructorDeclarationComparer : IComparer<ConstructorDeclarationSyntax>
{
    /// <summary> The default instance. </summary>
    public static readonly ConstructorDeclarationComparer Default = new();

    /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
    /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
    /// <param name="x">The first node to compare.</param>
    /// <param name="y">The second node to compare.</param>
    public static int Compare(ConstructorDeclarationSyntax? x, ConstructorDeclarationSyntax? y)
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

        var compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
        if (compare != 0)
        {
            return compare;
        }

        compare = MemberDeclarationComparer.CompareAccessibility(x.Modifiers, y.Modifiers, Accessibility.Private);
        if (compare != 0)
        {
            return compare;
        }

        return MemberDeclarationComparer.CompareSpanStart(x, y);
    }

    /// <inheritdoc />
    int IComparer<ConstructorDeclarationSyntax>.Compare(ConstructorDeclarationSyntax? x, ConstructorDeclarationSyntax? y) => Compare(x, y);
}

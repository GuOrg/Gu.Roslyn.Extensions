namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Collections.Generic;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// Helper methods for <see cref="SyntaxGenerator"/>.
    /// </summary>
    public static class SyntaxGeneratorExt
    {
        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="generator">The <see cref="SyntaxGenerator"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TypeDeclarationSyntax AddSorted(this SyntaxGenerator generator, TypeDeclarationSyntax containingType, MemberDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
        {
            if (generator is null)
            {
                throw new System.ArgumentNullException(nameof(generator));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (member is null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            return containingType.AddSorted(member, comparer);
        }
    }
}

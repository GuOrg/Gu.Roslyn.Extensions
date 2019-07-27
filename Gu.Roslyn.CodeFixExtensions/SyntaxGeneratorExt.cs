namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    public static class SyntaxGeneratorExt
    {
        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="generator">The <see cref="SyntaxGenerator"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TypeDeclarationSyntax AddSorted(this SyntaxGenerator generator, TypeDeclarationSyntax containingType, MemberDeclarationSyntax member)
        {
            for (var i = 0; i < containingType.Members.Count; i++)
            {
                var existing = containingType.Members[i];
                if (MemberDeclarationComparer.Compare(member, existing) < 0)
                {
                    if (!member.HasTrailingTrivia)
                    {
                        member = member.WithTrailingLineFeed();
                    }

                    if (containingType.Members.TryElementAt(i - 1, out var existingBefore) &&
                        ShouldAddLeadingLineFeed(existingBefore, member))
                    {
                        member = member.WithLeadingLineFeed();
                    }

                    if (ShouldAddLeadingLineFeed(member, existing))
                    {
                        containingType = (TypeDeclarationSyntax)generator.ReplaceNode(containingType, existing, existing.WithLeadingLineFeed());
                    }

                    return (TypeDeclarationSyntax)generator.InsertNodesBefore(containingType, containingType.Members[i], new[] { member });
                }
            }

            if (containingType.CloseBraceToken.LeadingTrivia.Any(x => x.IsDirective))
            {
                _ = (TypeDeclarationSyntax)generator.AddMembers(
                    containingType,
                    member.WithLeadingTrivia(
                        SyntaxFactory.TriviaList(
                            containingType.CloseBraceToken.LeadingTrivia.Where(x => x.IsDirective).Concat(new[] { SyntaxFactory.LineFeed }))));
                return containingType.ReplaceToken(
                    containingType.CloseBraceToken,
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(containingType.CloseBraceToken.LeadingTrivia.Where(x => !x.IsDirective)),
                        SyntaxKind.CloseBraceToken,
                        containingType.CloseBraceToken.TrailingTrivia));
            }

            if (containingType.Members.TryLast(out var last) &&
                ShouldAddLeadingLineFeed(last, member))
            {
                member = member.WithLeadingLineFeed();
            }

            return (TypeDeclarationSyntax)generator.AddMembers(containingType, member);
        }

        private static bool ShouldAddLeadingLineFeed(MemberDeclarationSyntax before, MemberDeclarationSyntax after)
        {
            if (after.HasLeadingTrivia &&
                after.GetLeadingTrivia() is SyntaxTriviaList leading &&
                leading.TryFirst(out var first) &&
                first.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return false;
            }

            if (after.IsKind(SyntaxKind.FieldDeclaration) &&
                before.IsKind(SyntaxKind.FieldDeclaration) &&
                MemberDeclarationComparer.Compare(before, after) == 0)
            {
                return false;
            }

            return true;
        }
    }
}

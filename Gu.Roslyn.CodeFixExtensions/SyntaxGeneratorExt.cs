namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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

                    if (TryMoveDirectives(existing.GetFirstToken(), member, out var token, out var memberWithDirectives))
                    {
                        containingType = (TypeDeclarationSyntax)generator.InsertNodesBefore(
                            containingType,
                            existing,
                            new[] { memberWithDirectives });
                        return containingType.ReplaceToken(containingType.Members[i + 1].GetFirstToken(), token);
                    }

                    if (containingType.Members.TryElementAt(i - 1, out var last) &&
                        ShouldAddLeadingLineFeed(last, member))
                    {
                        member = member.WithLeadingLineFeed();
                    }

                    if (ShouldAddLeadingLineFeed(member, existing))
                    {
                        containingType = (TypeDeclarationSyntax)generator.ReplaceNode(containingType, containingType.Members[i], existing.WithLeadingLineFeed());
                    }

                    return (TypeDeclarationSyntax)generator.InsertNodesBefore(containingType, containingType.Members[i], new[] { member });
                }
            }

            if (TryMoveDirectives(containingType.CloseBraceToken, member, out var closeBraceToken, out var memberWithDirective))
            {
                containingType = (TypeDeclarationSyntax)generator.AddMembers(
                    containingType,
                    memberWithDirective);
                return containingType.ReplaceToken(containingType.CloseBraceToken, closeBraceToken);
            }
            else
            {
                if (containingType.Members.TryLast(out var last) &&
                    ShouldAddLeadingLineFeed(last, member))
                {
                    member = member.WithLeadingLineFeed();
                }

                return (TypeDeclarationSyntax)generator.AddMembers(containingType, member);
            }
        }

        private static bool TryMoveDirectives<T>(SyntaxToken source, T target, out SyntaxToken updated, out T updatedTarget)
            where T : SyntaxNode
        {
            if (source.HasLeadingTrivia &&
                source.LeadingTrivia.Any(x => ShouldMove(x)))
            {
                updated = source.WithLeadingTrivia(source.LeadingTrivia.SkipWhile(x => ShouldMove(x)));
                var leading = source.LeadingTrivia.TakeWhile(x => ShouldMove(x)).Concat(new[] { SyntaxFactory.LineFeed }).ToArray();
                updatedTarget = target.WithLeadingTrivia(leading);
                return true;
            }

            updated = default;
            updatedTarget = default;
            return false;

            bool ShouldMove(SyntaxTrivia trivia)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.EndIfDirectiveTrivia:
                    case SyntaxKind.EndRegionDirectiveTrivia:
                    case SyntaxKind.PragmaWarningDirectiveTrivia when trivia.ToString().Contains("#pragma warning restore"):
                        return true;
                    default:
                        return false;
                }
            }
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

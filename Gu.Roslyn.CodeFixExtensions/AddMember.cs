namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for adding members.
    /// </summary>
    public static class AddMember
    {
        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <typeparam name="TMember">The type of <paramref name="member"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddSorted<TContaining, TMember>(this TContaining containingType, TMember member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
            where TMember : MemberDeclarationSyntax
        {
            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (member is null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            comparer ??= MemberDeclarationComparer.Default;
            if (!member.HasLeadingTrivia)
            {
                member = member.WithLeadingTrivia(SyntaxFactory.Whitespace(containingType.LeadingWhitespace() + "    "));
            }

#pragma warning disable CA1062 // Already checked
            if (!member.HasTrailingTrivia)
#pragma warning restore CA1062 // Validate arguments of public methods
            {
                member = member.WithTrailingLineFeed();
            }

            for (var i = 0; i < containingType.Members.Count; i++)
            {
                var existing = containingType.Members[i];
                if (comparer.Compare(member, existing) < 0)
                {
                    if (TryMoveDirectives(existing.GetFirstToken(), member, out var token, out var memberWithDirectives))
                    {
                        containingType = containingType.InsertNodesBefore(
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
                        containingType = containingType.ReplaceNode(containingType.Members[i], existing.WithLeadingLineFeed());
                    }

#pragma warning disable CA1062 /// Already checked
                    return containingType.InsertNodesBefore(containingType.Members[i], new[] { member });
#pragma warning restore CA1062 // Validate arguments of public methods
                }
            }

            if (TryMoveDirectives(containingType.CloseBraceToken, member, out var closeBraceToken, out var memberWithDirective))
            {
                containingType = (TContaining)containingType.AddMembers(memberWithDirective);
                return containingType.ReplaceToken(containingType.CloseBraceToken, closeBraceToken);
            }
            else
            {
                if (containingType.Members.TryLast(out var last) &&
                    ShouldAddLeadingLineFeed(last, member))
                {
                    member = member.WithLeadingLineFeed();
                }

                return (TContaining)containingType.AddMembers(member);
            }
        }

        private static bool TryMoveDirectives<T>(SyntaxToken source, T target, out SyntaxToken updated, [NotNullWhen(true)] out T? updatedTarget)
            where T : SyntaxNode
        {
            if (source.HasLeadingTrivia &&
                source.LeadingTrivia.Any(x => ShouldMove(x)))
            {
                updated = source.WithLeadingTrivia(source.LeadingTrivia.SkipWhile(x => ShouldMove(x)));
                updatedTarget = target.PrependLeadingTrivia(source.LeadingTrivia.TakeWhile(x => ShouldMove(x)).Concat(new[] { SyntaxFactory.LineFeed }));
                return true;
            }

            updated = default;
            updatedTarget = default;
            return false;

            static bool ShouldMove(SyntaxTrivia trivia)
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
            if (after.TryGetLeadingNewLine(out _))
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

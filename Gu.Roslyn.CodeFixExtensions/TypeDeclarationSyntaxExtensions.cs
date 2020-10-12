namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for adding members.
    /// </summary>
    public static class TypeDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddField<TContaining>(this TContaining containingType, FieldDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
        {
            return AddSorted(containingType, member, comparer);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddEvent<TContaining>(this TContaining containingType, EventDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
        {
            return AddSorted(containingType, member, comparer);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddEvent<TContaining>(this TContaining containingType, EventFieldDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
        {
            return AddSorted(containingType, member, comparer);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddProperty<TContaining>(this TContaining containingType, PropertyDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
        {
            return AddSorted(containingType, member, comparer);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining AddMethod<TContaining>(this TContaining containingType, MethodDeclarationSyntax member, IComparer<MemberDeclarationSyntax>? comparer = null)
            where TContaining : TypeDeclarationSyntax
        {
            return AddSorted(containingType, member, comparer);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <typeparam name="TMember">The type of <paramref name="member"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <typeparamref name="TMember"/>=.</param>
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

            var temp = (TContaining)containingType.AddMembers(member);
            var last = (TMember)temp.Members.Last();
            comparer ??= MemberDeclarationComparer.Default;
            for (var i = 0; i < temp.Members.Count - 1; i++)
            {
                if (comparer.Compare(last, temp.Members[i]) < 0)
                {
                    return containingType.InsertMember(member, i);
                }
            }

            return containingType.InsertMember(member, containingType.Members.Count);
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <typeparam name="TMember">The type of <paramref name="member"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <typeparamref name="TMember"/>=.</param>
        /// <param name="index">The index to insert at.</param>
        /// <returns>The <paramref name="containingType"/> with <paramref name="member"/>.</returns>
        public static TContaining InsertMember<TContaining, TMember>(this TContaining containingType, TMember member, int index)
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

            if (NextToken() is { HasLeadingTrivia: true } token &&
                token.LeadingTrivia.Any(x => IsEndDirective(x)))
            {
                containingType = containingType.ReplaceToken(token, token.WithLeadingTrivia(token.LeadingTrivia.SkipWhile(x => IsEndDirective(x))));
                member = member.PrependLeadingTrivia(token.LeadingTrivia.TakeWhile(x => IsEndDirective(x)).Concat(new[] { SyntaxFactory.LineFeed }));
            }

            if (!member.TryGetLeadingWhitespace(out _))
            {
                var leadingWhiteSpace = containingType.TryGetLeadingWhitespace(out var classWhitespace)
                    ? classWhitespace + "    "
                    : "            ";

                member = WithLeadingWhiteSpace(member, SyntaxFactory.Whitespace(leadingWhiteSpace));
            }

            if (index > 0 &&
                ShouldAddLeadingLineFeed(containingType.Members[index - 1], member))
            {
                member = WithLeadingNewLine(member);
            }

            if (!member.TryGetTrailingNewLine(out _))
            {
                member = member.AppendTrailingTrivia(SyntaxFactory.LineFeed);
            }

            if (containingType.Members.TryElementAt(index, out var existing) &&
                ShouldAddLeadingLineFeed(member, existing))
            {
                containingType = containingType.ReplaceNode(existing, existing.WithLeadingLineFeed());
            }

            return (TContaining)containingType.WithMembers(containingType.Members.Insert(index, member));

            static bool ShouldAddLeadingLineFeed(MemberDeclarationSyntax before, MemberDeclarationSyntax after)
            {
                if (after.TryGetLeadingNewLine(out _))
                {
                    return false;
                }

                if (after.IsKind(SyntaxKind.FieldDeclaration) &&
                    before.IsKind(SyntaxKind.FieldDeclaration) &&
                    MemberDeclarationComparer.CompareAccessibility(before.Modifiers, after.Modifiers, Accessibility.Private) == 0 &&
                    MemberDeclarationComparer.CompareScope(before.Modifiers, after.Modifiers) == 0 &&
                    CompareReadOnly(before.Modifiers, after.Modifiers) == 0 &&
                    !after.HasStructuredTrivia)
                {
                    return false;
                }

                if (after.HasLeadingTrivia &&
                    after.GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia))
                {
                    return false;
                }

                return true;

                static int CompareReadOnly(SyntaxTokenList x, SyntaxTokenList y)
                {
                    return Index(x).CompareTo(Index(y));

                    static int Index(SyntaxTokenList modifiers)
                    {
                        return modifiers.Any(SyntaxKind.ReadOnlyKeyword) ? 0 : 1;
                    }
                }
            }

            SyntaxToken NextToken()
            {
                if (index == containingType.Members.Count)
                {
                    return containingType.GetLastToken();
                }

                return containingType.Members[index].GetFirstToken();
            }

            static bool IsEndDirective(SyntaxTrivia trivia)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.EndIfDirectiveTrivia:
                    case SyntaxKind.PragmaWarningDirectiveTrivia when trivia.ToString().Contains("#pragma warning restore"):
                        return true;
                    default:
                        return false;
                }
            }

            static T WithLeadingWhiteSpace<T>(T member, SyntaxTrivia whitespace)
                  where T : MemberDeclarationSyntax
            {
                if (!member.HasLeadingTrivia)
                {
                    return member.WithLeadingTrivia(whitespace);
                }

                return member.WithLeadingTrivia(Update(member.GetLeadingTrivia()));

                IEnumerable<SyntaxTrivia> Update(SyntaxTriviaList leadingTrivia)
                {
                    foreach (var trivia in leadingTrivia)
                    {
                        if (IsEndDirective(trivia))
                        {
                            yield return trivia;
                        }
                        else if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            yield return trivia;
                            yield return whitespace;
                        }
                        else if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                        {
                            yield return whitespace;
                            if (trivia.HasStructure)
                            {
                                yield return CommentRewriter.Indent(trivia, whitespace.ToString());
                            }
                            else
                            {
                                yield return trivia;
                            }

                            yield return whitespace;
                        }
                        else
                        {
                            yield return trivia;
                        }
                    }
                }
            }

            static T WithLeadingNewLine<T>(T member)
                 where T : MemberDeclarationSyntax
            {
                if (!member.HasLeadingTrivia)
                {
                    return member.WithLeadingTrivia(SyntaxFactory.LineFeed);
                }

                var leadingTrivia = member.GetLeadingTrivia();
                return member.WithLeadingTrivia(leadingTrivia.Insert(Index(), SyntaxFactory.LineFeed));

                int Index()
                {
                    for (var i = 0; i < leadingTrivia.Count; i++)
                    {
                        if (!IsEndDirective(leadingTrivia[i]))
                        {
                            return i;
                        }
                    }

                    return 0;
                }
            }
        }

        /// <summary>
        /// Add the member and respect StyleCop ordering.
        /// </summary>
        /// <typeparam name="TContaining">The type of <paramref name="containingType"/>.</typeparam>
        /// <param name="containingType">The containing type.</param>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <returns>The <paramref name="containingType"/> with moved member.</returns>
        public static TContaining MoveMember<TContaining>(this TContaining containingType, int oldIndex, int newIndex)
            where TContaining : TypeDeclarationSyntax
        {
            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            return (TContaining)containingType.WithMembers(containingType.Members.Move(oldIndex, newIndex));
        }

        private class CommentRewriter : CSharpSyntaxRewriter
        {
            private readonly string whitespace;
            private int n;

            private CommentRewriter(string whitespace)
                : base(visitIntoStructuredTrivia: true)
            {
                this.whitespace = whitespace;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.DocumentationCommentExteriorTrivia
                        when trivia.ToString() is {} text &&
                             text.StartsWith("///", StringComparison.Ordinal):
                        this.n++;
                        return this.n == 1
                            ? base.VisitTrivia(trivia)
                            : SyntaxFactory.DocumentationCommentExterior(this.whitespace + text);
                    default:
                        return base.VisitTrivia(trivia);
                }
            }

            internal static SyntaxTrivia Indent(SyntaxTrivia trivia, string whitespace)
            {
                return new CommentRewriter(whitespace).VisitTrivia(trivia);
            }
        }
    }
}

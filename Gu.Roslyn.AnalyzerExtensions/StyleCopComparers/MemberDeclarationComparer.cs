namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// For comparing order according to StyleCop.
    /// </summary>
    public sealed class MemberDeclarationComparer : IComparer<MemberDeclarationSyntax>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly MemberDeclarationComparer Default = new MemberDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
        public static int Compare(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y)
        {
            if (TryCompare<FieldDeclarationSyntax>(x, y, FieldDeclarationComparer.Compare, out var result) ||
                TryCompare<ConstructorDeclarationSyntax>(x, y, ConstructorDeclarationComparer.Compare, out result) ||
                TryCompareEvent(x, y, out result) ||
                TryCompare<EnumDeclarationSyntax>(x, y, EnumDeclarationComparer.Compare, out result) ||
                TryCompare<PropertyDeclarationSyntax>(x, y, PropertyDeclarationComparer.Compare, out result) ||
                TryCompare<IndexerDeclarationSyntax>(x, y, IndexerDeclarationComparer.Compare, out result) ||
                TryCompare<MethodDeclarationSyntax>(x, y, MethodDeclarationComparer.Compare, out result) ||
                TryCompare<StructDeclarationSyntax>(x, y, StructDeclarationComparer.Compare, out result) ||
                TryCompare<ClassDeclarationSyntax>(x, y, ClassDeclarationComparer.Compare, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Compare const &lt; static &lt; member.
        /// </summary>
        /// <param name="x">The first modifiers.</param>
        /// <param name="y">The other modifiers.</param>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        public static int CompareScope(SyntaxTokenList x, SyntaxTokenList y)
        {
            return Index(x).CompareTo(Index(y));

            static int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.ConstKeyword))
                {
                    return 0;
                }

                if (list.Any(SyntaxKind.StaticKeyword))
                {
                    return 1;
                }

                return 2;
            }
        }

        /// <summary>
        /// Compare public &lt; internal &lt; protected.
        /// </summary>
        /// <param name="x">The first modifiers.</param>
        /// <param name="y">The other modifiers.</param>
        /// <param name="default">The default value when missing.</param>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        public static int CompareAccessibility(SyntaxTokenList x, SyntaxTokenList y, Accessibility @default)
        {
            return CompareAccessibility(
                x.Accessibility(@default),
                y.Accessibility(@default));
        }

        /// <summary>
        /// Compare public &lt; internal &lt; protected.
        /// </summary>
        /// <param name="x">The first modifiers.</param>
        /// <param name="y">The other modifiers.</param>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        public static int CompareAccessibility(Accessibility x, Accessibility y)
        {
            return Index(x).CompareTo(Index(y));

            static int Index(Accessibility accessibility)
            {
                return accessibility switch
                {
                    Accessibility.Public => 0,
                    Accessibility.Internal => 1,
                    Accessibility.ProtectedAndInternal => 2,
                    Accessibility.Protected => 3,
                    Accessibility.Private => 4,
                    _ => 5,
                };
            }
        }

        /// <summary>
        /// Compare current document order.
        /// If the node is not in a document zero is returned.
        /// </summary>
        /// <param name="x">The first modifiers.</param>
        /// <param name="y">The other modifiers.</param>
        /// <returns>A signed integer that indicates if the node is before or after the other in the document.</returns>
        public static int CompareSpanStart(SyntaxNode x, SyntaxNode y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.SyntaxTree is { FilePath: { } xPath } &&
                y.SyntaxTree is { FilePath: { } yPath } &&
                xPath == yPath)
            {
                return x.SpanStart.CompareTo(y.SpanStart);
            }

            return 0;
        }

        /// <summary>
        /// Compare using <paramref name="compare"/> if <paramref name="x"/> and <paramref name="y"/> are of type <typeparamref name="T"/>.
        /// Return -1 if <paramref name="x"/> is of type <typeparamref name="T"/> and <paramref name="y"/> is not.
        /// Return +1 if <paramref name="y"/> is of type <typeparamref name="T"/> and <paramref name="x"/> is not.
        /// Return 0 if neither of <paramref name="x"/> nor <paramref name="y"/> is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="x">The first <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="y">The other <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="compare">The Func{T, T, int}.</param>
        /// <param name="result">The <see cref="int"/>.</param>
        /// <returns>
        /// <paramref name="compare"/> if <paramref name="x"/> and <paramref name="y"/> are of type <typeparamref name="T"/>.
        /// Return -1 if <paramref name="x"/> is of type <typeparamref name="T"/> and <paramref name="y"/> is not.
        /// Return +1 if <paramref name="y"/> is of type <typeparamref name="T"/> and <paramref name="x"/> is not.
        /// Return 0 if neither of <paramref name="x"/> nor <paramref name="y"/> is of type <typeparamref name="T"/>.
        /// </returns>
        public static bool TryCompare<T>(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y, Func<T, T, int> compare, out int result)
            where T : MemberDeclarationSyntax
        {
            if (compare is null)
            {
                throw new ArgumentNullException(nameof(compare));
            }

            if (x is T xt)
            {
                result = y is T yt ? compare(xt, yt) : -1;
                return true;
            }

            if (y is T)
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Compare using StyleCop rules if <paramref name="x"/> and <paramref name="y"/> are event declarations.
        /// Return -1 if <paramref name="x"/> is an event declaration and <paramref name="y"/> is not.
        /// Return +1 if <paramref name="y"/> is an event declaration and <paramref name="x"/> is not.
        /// Return 0 if neither of <paramref name="x"/> nor <paramref name="y"/> is an event declaration.
        /// </summary>
        /// <param name="x">The first <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="y">The other <see cref="MemberDeclarationSyntax"/>.</param>
        /// <param name="result">The <see cref="int"/>.</param>
        /// <returns>
        /// StyleCop rules if <paramref name="x"/> and <paramref name="y"/> are an event declaration.
        /// Return -1 if <paramref name="x"/> is an event declaration and <paramref name="y"/> is not.
        /// Return +1 if <paramref name="y"/> is an event declaration and <paramref name="x"/> is not.
        /// Return 0 if neither of <paramref name="x"/> nor <paramref name="y"/> is an event declaration.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Analysis wrong here.")]
        public static bool TryCompareEvent(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y, out int result)
        {
            if (IsEvent(x))
            {
                if (IsEvent(y))
                {
#pragma warning disable CS8604 // Possible null reference argument. bug in Roslyn
                    result = CompareAccessibility(Accessibility(x), Accessibility(y));
                    if (result != 0)
                    {
                        return true;
                    }

                    result = CompareScope(Modifiers(x), Modifiers(y));
                    if (result != 0)
                    {
                        return true;
                    }

                    result = CompareSpanStart(x, y);
#pragma warning restore CS8604 // Possible null reference argument.
                    return true;
                }

                result = -1;
                return true;
            }

            if (IsEvent(y))
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;

            static bool IsEvent(MemberDeclarationSyntax? candidate) => candidate is EventDeclarationSyntax ||
                                                                       candidate is EventFieldDeclarationSyntax;

            static Accessibility Accessibility(MemberDeclarationSyntax member)
            {
                switch (member)
                {
                    case EventDeclarationSyntax eventDeclaration:
                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Microsoft.CodeAnalysis.Accessibility.Public;
                        }

                        return eventDeclaration.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
                    case EventFieldDeclarationSyntax eventField:
                        return eventField.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
                    default:
                        return Microsoft.CodeAnalysis.Accessibility.NotApplicable;
                }
            }

            static SyntaxTokenList Modifiers(MemberDeclarationSyntax member)
            {
                return member switch
                {
                    EventDeclarationSyntax eventDeclaration => eventDeclaration.Modifiers,
                    EventFieldDeclarationSyntax eventField => eventField.Modifiers,
                    _ => default,
                };
            }
        }

        /// <inheritdoc/>
        int IComparer<MemberDeclarationSyntax>.Compare(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y) => Compare(x, y);
    }
}

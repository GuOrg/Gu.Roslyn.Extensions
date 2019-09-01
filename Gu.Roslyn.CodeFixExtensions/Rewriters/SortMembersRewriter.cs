namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Rewrite <see cref="ClassDeclarationSyntax"/> so that members are sorted according to how StyleCop wants it.
    /// </summary>
    public sealed class SortMembersRewriter : CSharpSyntaxRewriter
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private static readonly SortMembersRewriter Default = new SortMembersRewriter();
#pragma warning disable IDISP002, IDISP006
        private readonly ThreadLocal<ImmutableArray<MemberDeclarationSyntax>> sortedMembers = new ThreadLocal<ImmutableArray<MemberDeclarationSyntax>>();
        private readonly ThreadLocal<int> index = new ThreadLocal<int>();
#pragma warning restore IDISP006, IDISP002

        /// <summary>
        /// Rewrite <paramref name="typeDeclaration"/> so that members are sorted according to how StyleCop wants it.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="typeDeclaration"/>.</typeparam>
        /// <param name="typeDeclaration">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns><paramref name="typeDeclaration"/> with members sorted according to how StyleCop wants it.</returns>
        public static T Sort<T>(T typeDeclaration, IComparer<MemberDeclarationSyntax> comparer)
            where T : TypeDeclarationSyntax
        {
            if (typeDeclaration == null)
            {
                throw new ArgumentNullException(nameof(typeDeclaration));
            }

            return Default.SortCore(typeDeclaration, comparer);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) => this.Next();

        /// <inheritdoc />
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) => this.Next();

        private SyntaxNode Next()
        {
            var next = this.sortedMembers.Value[this.index.Value];
            this.index.Value++;
            return next;
        }

        private T SortCore<T>(T typeDeclaration, IComparer<MemberDeclarationSyntax> comparer)
            where T : TypeDeclarationSyntax
        {
            if (typeDeclaration.Members.Count == 0)
            {
                return typeDeclaration;
            }

            var sorted = typeDeclaration.Members.OrderBy(x => x, comparer ?? MemberDeclarationComparer.Default).ToArray();
            for (int i = 0; i < sorted.Length; i++)
            {
                sorted[i] = i == 0 ? sorted[i].AdjustLeadingNewLine(null) : sorted[i].AdjustLeadingNewLine(sorted[i - 1]);
            }

            this.sortedMembers.Value = sorted.ToImmutableArray();
            this.index.Value = 0;
            switch (typeDeclaration)
            {
                case ClassDeclarationSyntax declaration:
                    typeDeclaration = (T)base.VisitClassDeclaration(declaration);
                    break;
                case StructDeclarationSyntax declaration:
                    typeDeclaration = (T)base.VisitStructDeclaration(declaration);
                    break;
            }

            if (this.index.Value != this.sortedMembers.Value.Length)
            {
                throw new InvalidOperationException("Bug in Gu.Roslyn.Asserts. All members were not sorted.");
            }

            this.sortedMembers.Value = ImmutableArray<MemberDeclarationSyntax>.Empty;
            return typeDeclaration;
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Helpers for working with <see cref="DocumentEditor"/>.
    /// </summary>
    public static partial class DocumentEditorExt
    {
        /// <summary>
        /// Same as DocumentEditor.ReplaceNode but nicer types.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="replacement">The replacement factory.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor ReplaceNode<T>(this DocumentEditor editor, T node, Func<T, SyntaxNode> replacement)
            where T : SyntaxNode
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (replacement == null)
            {
                throw new ArgumentNullException(nameof(replacement));
            }

            editor.ReplaceNode(node, (x, _) => replacement((T)x));
            return editor;
        }

        /// <summary>
        /// Same as DocumentEditor.ReplaceNode but nicer types.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="oldToken">The <see cref="SyntaxToken"/>.</param>
        /// <param name="newToken">The new <see cref="SyntaxToken"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor ReplaceToken(this DocumentEditor editor, SyntaxToken oldToken, SyntaxToken newToken)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            editor.ReplaceNode(oldToken.Parent, oldToken.Parent.ReplaceToken(oldToken, newToken));
            return editor;
        }

        /// <summary>
        /// Move <paramref name="toMove"></paramref> before <paramref name="member">.</paramref>.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="toMove">The <see cref="MemberDeclarationSyntax"/> to move.</param>
        /// <param name="member">The new <see cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor MoveBefore(this DocumentEditor editor, MemberDeclarationSyntax toMove, MemberDeclarationSyntax member)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (toMove == null)
            {
                throw new ArgumentNullException(nameof(toMove));
            }

            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            editor.RemoveNode(toMove);
            if (member.Parent is TypeDeclarationSyntax typeDeclaration &&
                typeDeclaration.Members.IndexOf(member) == 0)
            {
                editor.InsertBefore(member, new[] { toMove.WithLeadingElasticLineFeed() });
                editor.ReplaceNode(member, member.WithLeadingElasticLineFeed());
            }
            else
            {
                editor.InsertBefore(member, new[] { toMove });
            }

            return editor;
        }

        /// <summary>
        /// Move <paramref name="toMove"></paramref> before <paramref name="member">.</paramref>.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="toMove">The <see cref="MemberDeclarationSyntax"/> to move.</param>
        /// <param name="member">The new <see cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor MoveAfter(this DocumentEditor editor, MemberDeclarationSyntax toMove, MemberDeclarationSyntax member)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (toMove == null)
            {
                throw new ArgumentNullException(nameof(toMove));
            }

            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            editor.RemoveNode(toMove);
            if (toMove.Parent is TypeDeclarationSyntax typeDeclaration &&
                typeDeclaration.Members.IndexOf(toMove) == 0)
            {
                editor.InsertAfter(member, new[] { toMove.WithLeadingElasticLineFeed() });
                editor.ReplaceNode(member, member.WithLeadingElasticLineFeed());
            }
            else
            {
                editor.InsertAfter(member, new[] { toMove });
            }

            return editor;
        }

        /// <summary>
        /// Move <paramref name="toMove"></paramref> before <paramref name="statement">.</paramref>.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="toMove">The <see cref="StatementSyntax"/> to move.</param>
        /// <param name="statement">The new <see cref="StatementSyntax"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor MoveBefore(this DocumentEditor editor, StatementSyntax toMove, StatementSyntax statement)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (toMove == null)
            {
                throw new ArgumentNullException(nameof(toMove));
            }

            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            editor.RemoveNode(toMove);
            editor.InsertBefore(statement, new[] { toMove });
            return editor;
        }

        /// <summary>
        /// Move <paramref name="toMove"></paramref> before <paramref name="statement">.</paramref>.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="toMove">The <see cref="StatementSyntax"/> to move.</param>
        /// <param name="statement">The new <see cref="StatementSyntax"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor MoveAfter(this DocumentEditor editor, StatementSyntax toMove, StatementSyntax statement)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (toMove == null)
            {
                throw new ArgumentNullException(nameof(toMove));
            }

            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            editor.RemoveNode(toMove);
            editor.InsertAfter(statement, new[] { toMove });
            return editor;
        }

        /// <summary>
        /// Add <see cref="Formatter.Annotation"/> to <paramref name="node"/>.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor FormatNode(this DocumentEditor editor, SyntaxNode node)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            editor.ReplaceNode(node, (x, _) => x.WithAdditionalAnnotations(Formatter.Annotation));
            return editor;
        }

        /// <summary>
        /// Rewrite <paramref name="classDeclaration"/> to sealed.
        /// Change protected -> private
        /// Remove virtual.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor Seal(this DocumentEditor editor, ClassDeclarationSyntax classDeclaration)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (classDeclaration == null)
            {
                throw new ArgumentNullException(nameof(classDeclaration));
            }

            if (classDeclaration.Modifiers.Any(SyntaxKind.SealedKeyword))
            {
                return editor;
            }

            editor.ReplaceNode(classDeclaration, x => SealRewriter.Seal(x));
            return editor;
        }

        /// <summary>
        /// Rewrite <paramref name="classDeclaration"/> to sealed.
        /// Change protected -> private
        /// Remove virtual.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{MemberDeclarationSyntax}"/>. If null <see cref="MemberDeclarationComparer.Default"/> is used.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor SortMembers(this DocumentEditor editor, TypeDeclarationSyntax classDeclaration, IComparer<MemberDeclarationSyntax> comparer = null)
        {
            if (editor == null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            if (classDeclaration == null)
            {
                throw new ArgumentNullException(nameof(classDeclaration));
            }

            editor.ReplaceNode(classDeclaration, x => SortMembersRewriter.Sort(x, comparer));
            return editor;
        }
    }
}

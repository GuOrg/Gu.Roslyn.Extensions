namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
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
        public static DocumentEditor MakeSealed(this DocumentEditor editor, ClassDeclarationSyntax classDeclaration)
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

            editor.ReplaceNode(classDeclaration, (node, generator) => MakeSealedRewriter.Default.Visit(node, (ClassDeclarationSyntax)node));
            return editor;
        }

        private class MakeSealedRewriter : CSharpSyntaxRewriter
        {
            internal static readonly MakeSealedRewriter Default = new MakeSealedRewriter();

            private static readonly ThreadLocal<ClassDeclarationSyntax> CurrentClass = new ThreadLocal<ClassDeclarationSyntax>();

            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // We only want to make the top level class sealed.
                if (ReferenceEquals(CurrentClass.Value, node))
                {
                    var updated = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);
                    return updated.WithModifiers(updated.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.SealedKeyword)));
                }

                return node;
            }

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (TryUpdate(node.Modifiers, out var modifiers))
                {
                    return node.WithModifiers(modifiers);
                }

                return node;
            }

            public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
            {
                if (TryUpdate(node.Modifiers, out var modifiers))
                {
                    node = node.WithModifiers(modifiers);
                }

                return base.VisitEventDeclaration(node);
            }

            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (TryUpdate(node.Modifiers, out var modifiers))
                {
                    node = node.WithModifiers(modifiers);
                }

                return base.VisitPropertyDeclaration(node);
            }

            public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
            {
                if (node.TryFirstAncestor(out BasePropertyDeclarationSyntax parent) &&
                    parent.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                    node.Modifiers.TrySingle(x => x.IsKind(SyntaxKind.PrivateKeyword), out var modifier))
                {
                    return node.WithModifiers(node.Modifiers.Remove(modifier));
                }

                return TryUpdate(node.Modifiers, out var modifiers)
                    ? node.WithModifiers(modifiers)
                    : node;
            }

            public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                if (TryUpdate(node.Modifiers, out var modifiers))
                {
                    return node.WithModifiers(modifiers);
                }

                return node;
            }

            internal SyntaxNode Visit(SyntaxNode node, ClassDeclarationSyntax classDeclaration)
            {
                CurrentClass.Value = classDeclaration;
                var updated = this.Visit(node);
                CurrentClass.Value = null;
                return updated;
            }

            private static bool TryUpdate(SyntaxTokenList modifiers, out SyntaxTokenList result)
            {
                result = modifiers;
                if (modifiers.TrySingle(x => x.IsKind(SyntaxKind.VirtualKeyword), out var modifier))
                {
                    result = modifiers.Remove(modifier);
                }

                if (result.TrySingle(x => x.IsKind(SyntaxKind.ProtectedKeyword), out modifier))
                {
                    result = result.Replace(modifier, SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                }

                return result != modifiers;
            }
        }
    }
}

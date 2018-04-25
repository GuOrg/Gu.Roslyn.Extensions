namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Helpers for working with <see cref="DocumentEditor"/>
    /// </summary>
    public static partial class DocumentEditorExt
    {
        /// <summary>
        /// Same as DocumentEditor.ReplaceNode but nicer types.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="replacement">The replacement factory.</param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor ReplaceNode<T>(this DocumentEditor editor, T node, Func<T, SyntaxNode> replacement)
            where T : SyntaxNode
        {
            editor.ReplaceNode(node, (x, _) => replacement((T)x));
            return editor;
        }

        /// <summary>
        /// Add <see cref="Formatter.Annotation"/> to <paramref name="node"/>
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <returns>The <see cref="DocumentEditor"/> that was passed in.</returns>
        public static DocumentEditor FormatNode(this DocumentEditor editor, SyntaxNode node)
        {
            if (node == null)
            {
                return editor;
            }

            editor.ReplaceNode(node, (x, _) => x.WithAdditionalAnnotations(Formatter.Annotation));
            return editor;
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;

    public static partial class DocumentEditorExt
    {
        public static DocumentEditor ReplaceNode<T>(this DocumentEditor editor, T node, Func<T, T> replacement)
            where T : SyntaxNode
        {
            editor.ReplaceNode(node, (x, _) => replacement((T)x));
            return editor;
        }

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

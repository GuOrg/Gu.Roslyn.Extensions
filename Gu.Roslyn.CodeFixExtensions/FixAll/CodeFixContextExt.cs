namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// Helper methods for registering <see cref="DocumentEditorAction"/>
    /// </summary>
    internal static class CodeFixContextExt
    {
        public static void RegisterDocumentEditorFix(
            this CodeFixContext context,
            string title,
            Action<DocumentEditor, CancellationToken> action,
            Diagnostic diagnostic)
        {
            RegisterDocumentEditorFix(context, title, action, title, diagnostic);
        }

        // ReSharper disable once UnusedMember.Global
        public static void RegisterDocumentEditorFix(
            this CodeFixContext context,
            string title,
            Action<DocumentEditor, CancellationToken> action,
            Type equivalenceKey,
            Diagnostic diagnostic)
        {
            context.RegisterCodeFix(
                new DocumentEditorAction(title, context.Document, action, equivalenceKey.FullName),
                diagnostic);
        }

        public static void RegisterDocumentEditorFix(
            this CodeFixContext context,
            string title,
            Action<DocumentEditor, CancellationToken> action,
            string equivalenceKey,
            Diagnostic diagnostic)
        {
            context.RegisterCodeFix(
                new DocumentEditorAction(title, context.Document, action, equivalenceKey),
                diagnostic);
        }
    }
}

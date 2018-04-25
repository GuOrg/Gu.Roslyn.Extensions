#pragma warning disable GU0008 // Avoid relay properties.
namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// A CodeFixContext for usage with <see cref="DocumentEditorCodeFixProvider"/>
    /// </summary>
    public struct DocumentEditorCodeFixContext
    {
        private readonly CodeFixContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentEditorCodeFixContext"/> struct.
        /// </summary>
        /// <param name="context">The <see cref="CodeFixContext"/></param>
        public DocumentEditorCodeFixContext(CodeFixContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the document corresponding to the <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Span" /> to fix.
        /// </summary>
        public Document Document => this.context.Document;

        /// <summary>
        /// Gets the <see cref="CancellationToken"/>
        /// </summary>
        public CancellationToken CancellationToken => this.context.CancellationToken;

        /// <summary>
        /// Gets the text span within the <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Document" /> to fix.
        /// </summary>
        public TextSpan Span => this.context.Span;

        /// <summary>
        /// Gets the diagnostics to fix.
        /// NOTE: All the diagnostics in this collection have the same <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Span" />.
        /// </summary>
        public ImmutableArray<Diagnostic> Diagnostics => this.context.Diagnostics;

        /// <summary>
        /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
        /// </summary>
        /// <param name="title">Title of the <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
        /// <param name="action">The <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
        /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="P:Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
        /// <param name="diagnostic">The subset of <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
        public void RegisterCodeFix(string title, Action<DocumentEditor, CancellationToken> action, Type equivalenceKey, Diagnostic diagnostic)
        {
            this.context.RegisterCodeFix(
                new DocumentEditorAction(title, this.context.Document, action, equivalenceKey.FullName),
                diagnostic);
        }

        /// <summary>
        /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
        /// </summary>
        /// <param name="title">Title of the <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
        /// <param name="action">The <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
        /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="T:Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="P:Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
        /// <param name="diagnostic">The subset of <see cref="P:Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
        public void RegisterCodeFix(string title, Action<DocumentEditor, CancellationToken> action, string equivalenceKey, Diagnostic diagnostic)
        {
            this.context.RegisterCodeFix(
                new DocumentEditorAction(title, this.context.Document, action, equivalenceKey),
                diagnostic);
        }
    }
}

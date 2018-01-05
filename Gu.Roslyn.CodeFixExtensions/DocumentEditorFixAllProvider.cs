namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Editing;

    /// <inheritdoc />
    public class DocumentEditorFixAllProvider : FixAllProvider
    {
        /// <summary>
        /// For FixAllScope.Document
        /// </summary>
        public static readonly DocumentEditorFixAllProvider PerDocument = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document));

        /// <summary>
        /// For FixAllScope.Document and FixAllScope.Project
        /// </summary>
        public static readonly DocumentEditorFixAllProvider PerDocumentAndProject = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document, FixAllScope.Project));

        /// <summary>
        /// For FixAllScope.Document, FixAllScope.Project and FixAllScope.Solution
        /// </summary>
        public static readonly DocumentEditorFixAllProvider AllScopes = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution));

        private readonly ImmutableArray<FixAllScope> scopes;

        private DocumentEditorFixAllProvider(ImmutableArray<FixAllScope> scopes)
        {
            this.scopes = scopes;
        }

        /// <inheritdoc />
        public override IEnumerable<FixAllScope> GetSupportedFixAllScopes() => this.scopes;

        /// <inheritdoc />
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(fixAllContext.Document)
                                                 .ConfigureAwait(false);
            var documentEditorActions = new List<DocumentEditorAction>();
            foreach (var diagnostic in diagnostics)
            {
                var codeFixContext = new CodeFixContext(
                    fixAllContext.Document,
                    diagnostic,
                    (a, _) =>
                    {
                        if (a.EquivalenceKey == fixAllContext.CodeActionEquivalenceKey)
                        {
                            if (a is DocumentEditorAction docAction)
                            {
                                documentEditorActions.Add(docAction);
                            }
                            else
                            {
                                throw new InvalidOperationException("When using DocumentEditorFixAllProvider all registered code actions must be of type DocumentEditorAction");
                            }
                        }
                    },
                    fixAllContext.CancellationToken);
                await fixAllContext.CodeFixProvider.RegisterCodeFixesAsync(codeFixContext)
                                   .ConfigureAwait(false);
            }

            if (documentEditorActions.Count == 0)
            {
                return null;
            }

            return CodeAction.Create(documentEditorActions[0].Title, c => FixDocumentAsync(fixAllContext.Document, documentEditorActions, c));
        }

        private static async Task<Document> FixDocumentAsync(Document document, IReadOnlyList<DocumentEditorAction> documentEditorActions, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken)
                                             .ConfigureAwait(false);
            foreach (var action in documentEditorActions)
            {
                action.Action(editor, cancellationToken);
            }

            return editor.GetChangedDocument();
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// A <see cref="FixAllProvider"/> that uses <see cref="DocumentEditor"/> for batch changes.
    /// </summary>
    public sealed class DocumentEditorFixAllProvider : FixAllProvider
    {
        /// <summary>
        /// Fix all in document.
        /// </summary>
        public static readonly DocumentEditorFixAllProvider Document = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document));

        /// <summary>
        /// Fix all in project or document.
        /// </summary>
        public static readonly DocumentEditorFixAllProvider Project = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document, FixAllScope.Project));

        /// <summary>
        /// Fix all in solution, project or document.
        /// </summary>
        public static readonly DocumentEditorFixAllProvider Solution = new DocumentEditorFixAllProvider(ImmutableArray.Create(FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution));

        private readonly ImmutableArray<FixAllScope> supportedFixAllScopes;

        private DocumentEditorFixAllProvider(ImmutableArray<FixAllScope> supportedFixAllScopes)
        {
            this.supportedFixAllScopes = supportedFixAllScopes;
        }

        /// <inheritdoc />
        public override IEnumerable<FixAllScope> GetSupportedFixAllScopes() => this.supportedFixAllScopes;

        /// <inheritdoc />
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            if (fixAllContext == null)
            {
                throw new System.ArgumentNullException(nameof(fixAllContext));
            }

            if (fixAllContext.Scope == FixAllScope.Document)
            {
                var actions = await GetDocumentEditorActionsAsync(fixAllContext, fixAllContext.Document).ConfigureAwait(false);

                if (actions.Count == 0)
                {
                    return null;
                }

                return CodeAction.Create(actions[0].Title, c => FixDocumentAsync(fixAllContext.Document, actions, c));
            }

            if (fixAllContext.Scope == FixAllScope.Project)
            {
                var docActions = new Dictionary<Document, List<CodeAction>>();
                foreach (var document in fixAllContext.Project.Documents)
                {
                    var actions = await GetDocumentEditorActionsAsync(fixAllContext, document).ConfigureAwait(false);
                    if (actions.Count == 0)
                    {
                        continue;
                    }

                    docActions.Add(document, actions);
                }

                if (docActions.Count == 0)
                {
                    return null;
                }

                return CodeAction.Create(docActions.First().Value.First().Title, c => FixDocumentsAsync(fixAllContext.Solution, docActions, c));
            }

            if (fixAllContext.Scope == FixAllScope.Solution)
            {
                var docActions = new Dictionary<Document, List<CodeAction>>();
                foreach (var project in fixAllContext.Solution.Projects)
                {
                    foreach (var document in project.Documents)
                    {
                        var actions = await GetDocumentEditorActionsAsync(fixAllContext, document).ConfigureAwait(false);
                        if (actions.Count == 0)
                        {
                            continue;
                        }

                        docActions.Add(document, actions);
                    }
                }

                if (docActions.Count == 0)
                {
                    return null;
                }

                return CodeAction.Create(docActions.First().Value.First().Title, c => FixDocumentsAsync(fixAllContext.Solution, docActions, c));
            }

            return null;
        }

        private static async Task<List<CodeAction>> GetDocumentEditorActionsAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document)
                                                 .ConfigureAwait(false);
            var actions = new List<CodeAction>();
            foreach (var diagnostic in diagnostics)
            {
                var codeFixContext = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, _) =>
                    {
                        if (a.EquivalenceKey == fixAllContext.CodeActionEquivalenceKey)
                        {
                            actions.Add(a);
                        }
                    },
                    fixAllContext.CancellationToken);
                await fixAllContext.CodeFixProvider.RegisterCodeFixesAsync(codeFixContext)
                                   .ConfigureAwait(false);
            }

            return actions;
        }

        private static async Task<Document> FixDocumentAsync(Document document, IReadOnlyList<CodeAction> actions, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken)
                                             .ConfigureAwait(false);
            foreach (var action in actions)
            {
                switch (action)
                {
                    case DocumentEditorAction docAction:
                        docAction.Action(editor, cancellationToken);
                        break;
                    case AsyncDocumentEditorAction docAction:
                        await docAction.Action(editor, cancellationToken).ConfigureAwait(false);
                        break;
                }
            }

            return editor.GetChangedDocument();
        }

        private static async Task<Solution> FixDocumentsAsync(Solution solution, Dictionary<Document, List<CodeAction>> docActions, CancellationToken cancellationToken)
        {
            foreach (var docAction in docActions)
            {
                var document = await FixDocumentAsync(docAction.Key, docAction.Value, cancellationToken).ConfigureAwait(false);
                solution = solution.WithDocumentSyntaxRoot(
                    document.Id,
                    await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false));
            }

            return solution;
        }
    }
}

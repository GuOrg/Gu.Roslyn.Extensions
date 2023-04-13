namespace Gu.Roslyn.CodeFixExtensions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Editing;

/// <inheritdoc />
public class AsyncDocumentEditorAction : CodeAction
{
    private readonly Document document;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncDocumentEditorAction"/> class.
    /// </summary>
    /// <param name="title">The text to display to the user in visual studio.</param>
    /// <param name="document">The <see cref="Document"/>.</param>
    /// <param name="action">The action to perform to fix the diagnostic.</param>
    /// <param name="equivalenceKey">The key by which VS determines if actions should be used in the same batch.</param>
    public AsyncDocumentEditorAction(string title, Document document, Func<DocumentEditor, CancellationToken, Task> action, string? equivalenceKey)
    {
        this.document = document;
        this.Title = title;
        this.Action = action;
        this.EquivalenceKey = equivalenceKey;
    }

    /// <summary>
    /// Gets the action to perform to fix the diagnostic.
    /// </summary>
    public Func<DocumentEditor, CancellationToken, Task> Action { get; }

    /// <inheritdoc />
    public sealed override string Title { get; }

    /// <inheritdoc />
    public sealed override string? EquivalenceKey { get; }

    /// <inheritdoc />
    protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(this.document, cancellationToken)
                                         .ConfigureAwait(false);
        await this.Action(editor, cancellationToken).ConfigureAwait(false);
        return editor.GetChangedDocument();
    }
}

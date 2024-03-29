﻿namespace Gu.Roslyn.CodeFixExtensions;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// A CodeFixContext for usage with <see cref="DocumentEditorCodeFixProvider"/>.
/// </summary>
#pragma warning disable RS0016 // Add public types and members to the declared API
public readonly struct DocumentEditorCodeFixContext : IEquatable<DocumentEditorCodeFixContext>
#pragma warning restore RS0016 // Add public types and members to the declared API
{
    private readonly CodeFixContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentEditorCodeFixContext"/> struct.
    /// </summary>
    /// <param name="context">The <see cref="CodeFixContext"/>.</param>
    public DocumentEditorCodeFixContext(CodeFixContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets the document corresponding to the <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Span" /> to fix.
    /// </summary>
    public Document Document => this.context.Document;

    /// <summary>
    /// Gets the <see cref="CancellationToken"/>.
    /// </summary>
    public CancellationToken CancellationToken => this.context.CancellationToken;

    /// <summary>
    /// Gets the text span within the <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Document" /> to fix.
    /// </summary>
    public TextSpan Span => this.context.Span;

    /// <summary>
    /// Gets the diagnostics to fix.
    /// NOTE: All the diagnostics in this collection have the same <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Span" />.
    /// </summary>
    public ImmutableArray<Diagnostic> Diagnostics => this.context.Diagnostics;

    /// <summary>
    /// Check if <paramref name="left"/> is equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left <see cref="DocumentEditorCodeFixContext"/>.</param>
    /// <param name="right">The right <see cref="DocumentEditorCodeFixContext"/>.</param>
    /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
    public static bool operator ==(DocumentEditorCodeFixContext left, DocumentEditorCodeFixContext right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Check if <paramref name="left"/> is not equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left <see cref="DocumentEditorCodeFixContext"/>.</param>
    /// <param name="right">The right <see cref="DocumentEditorCodeFixContext"/>.</param>
    /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
    public static bool operator !=(DocumentEditorCodeFixContext left, DocumentEditorCodeFixContext right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
    /// </summary>
    /// <param name="title">Title of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
    /// <param name="action">The <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
    /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
    /// <param name="diagnostic">The subset of <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
    public void RegisterCodeFix(string title, Action<DocumentEditor, CancellationToken> action, Type equivalenceKey, Diagnostic diagnostic)
    {
        if (title is null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        if (equivalenceKey is null)
        {
            throw new ArgumentNullException(nameof(equivalenceKey));
        }

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        this.context.RegisterCodeFix(
            new DocumentEditorAction(title, this.context.Document, action, equivalenceKey.FullName ?? equivalenceKey.Name),
            diagnostic);
    }

    /// <summary>
    /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
    /// </summary>
    /// <param name="title">Title of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
    /// <param name="action">The <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
    /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
    /// <param name="diagnostic">The subset of <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
    public void RegisterCodeFix(string title, Action<DocumentEditor> action, string? equivalenceKey, Diagnostic diagnostic)
    {
        this.RegisterCodeFix(title, (e, _) => action(e), equivalenceKey, diagnostic);
    }

    /// <summary>
    /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
    /// </summary>
    /// <param name="title">Title of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
    /// <param name="action">The <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
    /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
    /// <param name="diagnostic">The subset of <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
    public void RegisterCodeFix(string title, Action<DocumentEditor, CancellationToken> action, string? equivalenceKey, Diagnostic diagnostic)
    {
        if (title is null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        this.context.RegisterCodeFix(
            new DocumentEditorAction(title, this.context.Document, action, equivalenceKey),
            diagnostic);
    }

    /// <summary>
    /// Add supplied <paramref name="action" /> to the list of fixes that will be offered to the user.
    /// </summary>
    /// <param name="title">Title of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />.</param>
    /// <param name="action">The <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> that will be invoked to apply the fix.</param>
    /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" /> with other <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction" />s. See <see cref="Microsoft.CodeAnalysis.CodeActions.CodeAction.EquivalenceKey" />.</param>
    /// <param name="diagnostic">The subset of <see cref="Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.Diagnostics" /> being addressed / fixed by the <paramref name="action" />.</param>
    public void RegisterCodeFix(string title, Func<DocumentEditor, CancellationToken, Task> action, string? equivalenceKey, Diagnostic diagnostic)
    {
        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        this.context.RegisterCodeFix(
            new AsyncDocumentEditorAction(title, this.context.Document, action, equivalenceKey),
            diagnostic);
    }

    /// <inheritdoc/>
    public bool Equals(DocumentEditorCodeFixContext other)
    {
        return this.context.Equals(other.context);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is DocumentEditorCodeFixContext other && this.Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.context.GetHashCode();
    }
}

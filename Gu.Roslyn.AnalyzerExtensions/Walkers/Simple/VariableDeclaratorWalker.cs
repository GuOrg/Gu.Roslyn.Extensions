namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Get all <see cref="VariableDeclaratorSyntax"/> in the scope.
/// </summary>
public sealed class VariableDeclaratorWalker : PooledWalker<VariableDeclaratorWalker>
{
    private readonly List<VariableDeclaratorSyntax> variableDeclarators = new();

    private VariableDeclaratorWalker()
    {
    }

    /// <summary>
    /// Gets the <see cref="VariableDeclaratorSyntax"/>s found in the scope.
    /// </summary>
    public IReadOnlyList<VariableDeclaratorSyntax> VariableDeclarators => this.variableDeclarators;

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static VariableDeclaratorWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new VariableDeclaratorWalker());

    /// <inheritdoc />
    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        this.variableDeclarators.Add(node);
        base.VisitVariableDeclarator(node);
    }

    /// <summary>
    /// Filters by <paramref name="match"/>.
    /// </summary>
    /// <param name="match">The predicate for finding items to remove.</param>
    public void RemoveAll(Predicate<VariableDeclaratorSyntax> match) => this.variableDeclarators.RemoveAll(match);

    /// <inheritdoc />
    protected override void Clear()
    {
        this.variableDeclarators.Clear();
    }
}

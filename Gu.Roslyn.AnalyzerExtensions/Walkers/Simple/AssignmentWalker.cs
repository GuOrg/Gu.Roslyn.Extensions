namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Find all <see cref="AssignmentExpressionSyntax"/> in the scope.
/// </summary>
public sealed class AssignmentWalker : PooledWalker<AssignmentWalker>
{
    private readonly List<AssignmentExpressionSyntax> assignments = new();

    private AssignmentWalker()
    {
    }

    /// <summary>
    /// Gets a list with all <see cref="AssignmentExpressionSyntax"/> in the scope.
    /// </summary>
    public IReadOnlyList<AssignmentExpressionSyntax> Assignments => this.assignments;

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static AssignmentWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new AssignmentWalker());

    /// <inheritdoc/>
    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        this.assignments.Add(node);
        base.VisitAssignmentExpression(node);
    }

    /// <summary>
    /// Filters by <paramref name="match"/>.
    /// </summary>
    /// <param name="match">The predicate for finding items to remove.</param>
    public void RemoveAll(Predicate<AssignmentExpressionSyntax> match) => this.assignments.RemoveAll(match);

    /// <inheritdoc/>
    protected override void Clear()
    {
        this.assignments.Clear();
    }
}

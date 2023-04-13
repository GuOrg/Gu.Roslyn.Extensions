namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Find all <see cref="ObjectCreationWalker"/> in the scope.
/// </summary>
public sealed class ObjectCreationWalker : PooledWalker<ObjectCreationWalker>
{
    private readonly List<ObjectCreationExpressionSyntax> objectCreations = new();

    private ObjectCreationWalker()
    {
    }

    /// <summary>
    /// Gets a collection with the <see cref="ObjectCreationExpressionSyntax"/> found when walking.
    /// </summary>
    public IReadOnlyList<ObjectCreationExpressionSyntax> ObjectCreations => this.objectCreations;

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static ObjectCreationWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new ObjectCreationWalker());

    /// <inheritdoc />
    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        this.objectCreations.Add(node);
        base.VisitObjectCreationExpression(node);
    }

    /// <summary>
    /// Filters by <paramref name="match"/>.
    /// </summary>
    /// <param name="match">The predicate for finding items to remove.</param>
    public void RemoveAll(Predicate<ObjectCreationExpressionSyntax> match) => this.objectCreations.RemoveAll(match);

    /// <inheritdoc/>
    protected override void Clear()
    {
        this.objectCreations.Clear();
    }
}

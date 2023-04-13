namespace Gu.Roslyn.AnalyzerExtensions;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Find all <see cref="LiteralExpressionSyntax"/> in the scope.
/// </summary>
public sealed class StringLiteralWalker : PooledWalker<StringLiteralWalker>
{
    private readonly List<LiteralExpressionSyntax> literals = new();

    private StringLiteralWalker()
    {
    }

    /// <summary>
    /// Gets a collection with the <see cref="LiteralExpressionSyntax"/> found when walking.
    /// </summary>
    public IReadOnlyList<LiteralExpressionSyntax> Literals => this.literals;

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static StringLiteralWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new StringLiteralWalker());

    /// <inheritdoc />
    public override void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            this.literals.Add(node);
        }

        base.VisitLiteralExpression(node);
    }

    /// <inheritdoc />
    protected override void Clear()
    {
        this.literals.Clear();
    }
}

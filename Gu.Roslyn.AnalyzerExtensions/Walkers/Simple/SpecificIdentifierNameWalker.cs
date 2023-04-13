namespace Gu.Roslyn.AnalyzerExtensions;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Find all <see cref="IdentifierNameSyntax"/> matching the name in the scope.
/// </summary>
public sealed class SpecificIdentifierNameWalker : PooledWalker<SpecificIdentifierNameWalker>
{
    private readonly List<IdentifierNameSyntax> identifierNames = new();
    private string valueText = null!;

    private SpecificIdentifierNameWalker()
    {
    }

    /// <summary>
    /// Gets a collection with the <see cref="IdentifierNameSyntax"/> found when walking.
    /// </summary>
    public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <param name="valueText">The name to match <see cref="SyntaxToken.ValueText"/> with.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static SpecificIdentifierNameWalker Borrow(SyntaxNode node, string valueText)
    {
        var walker = Borrow(() => new SpecificIdentifierNameWalker());
        walker.valueText = valueText;
        walker.Visit(node);
        return walker;
    }

    /// <inheritdoc />
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node is { Identifier.ValueText: { } candidate } &&
            candidate == this.valueText)
        {
            this.identifierNames.Add(node);
        }

        base.VisitIdentifierName(node);
    }

    /// <inheritdoc />
    protected override void Clear()
    {
        this.valueText = null!;
        this.identifierNames.Clear();
    }
}

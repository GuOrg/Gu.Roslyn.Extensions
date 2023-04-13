namespace Gu.Roslyn.AnalyzerExtensions;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Find all <see cref="UsingStatementSyntax"/> in the scope.
/// </summary>
public sealed class UsingStaticWalker : AbstractUsingDirectiveWalker<UsingStaticWalker>
{
    private readonly List<UsingDirectiveSyntax> usingDirectives = new();

    private UsingStaticWalker()
    {
    }

    /// <summary>
    /// Gets a collection with the <see cref="UsingDirectiveSyntax"/> found when walking.
    /// </summary>
    public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives => this.usingDirectives;

    /// <summary>
    /// Get a walker that has visited <paramref name="tree"/>.
    /// </summary>
    /// <param name="tree">The scope.</param>
    /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
    public static UsingStaticWalker Borrow(SyntaxTree tree)
    {
        if (tree is null)
        {
            throw new System.ArgumentNullException(nameof(tree));
        }

        if (tree.TryGetRoot(out var root))
        {
            return BorrowAndVisit(root, () => new UsingStaticWalker());
        }

        return Borrow(() => new UsingStaticWalker());
    }

    /// <summary>
    /// Get a walker that has visited <paramref name="tree"/>.
    /// </summary>
    /// <param name="tree">The scope.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
    public static async Task<UsingStaticWalker> BorrowAsync(SyntaxTree tree, CancellationToken cancellationToken)
    {
        if (tree is null)
        {
            throw new System.ArgumentNullException(nameof(tree));
        }

        var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        return BorrowAndVisit(root, () => new UsingStaticWalker());
    }

    /// <summary>
    /// Get a walker that has visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The scope.</param>
    /// <returns>A walker that has visited <paramref name="node"/>.</returns>
    public static UsingStaticWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new UsingStaticWalker());

    /// <summary>
    /// Try to get the type alias for the type name.
    /// </summary>
    /// <param name="tree">The <see cref="SyntaxTree"/>.</param>
    /// <param name="type">The type name. using Name = System.Type.</param>
    /// <param name="result">The alias if found.</param>
    /// <returns>True if an alias was found.</returns>
    public static bool TryGet(SyntaxTree tree, QualifiedType type, [NotNullWhen(true)] out UsingDirectiveSyntax? result)
    {
        if (tree is null)
        {
            throw new System.ArgumentNullException(nameof(tree));
        }

        if (type is null)
        {
            throw new System.ArgumentNullException(nameof(type));
        }

        if (tree.TryGetRoot(out var root))
        {
            using var walker = Borrow(root);
            foreach (var candidate in walker.usingDirectives)
            {
                if (candidate.Name == type)
                {
                    result = candidate;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    /// <inheritdoc />
    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        if (node is null)
        {
            throw new System.ArgumentNullException(nameof(node));
        }

        if (node.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
        {
            this.usingDirectives.Add(node);
        }

        base.VisitUsingDirective(node);
    }

    /// <inheritdoc/>
    protected override void Clear()
    {
        this.usingDirectives.Clear();
    }
}

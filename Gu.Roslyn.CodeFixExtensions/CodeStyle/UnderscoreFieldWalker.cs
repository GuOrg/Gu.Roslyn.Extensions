namespace Gu.Roslyn.CodeFixExtensions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Gu.Roslyn.AnalyzerExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Figures out if fields are named _filedName.
/// </summary>
public sealed class UnderscoreFieldWalker : CompilationStyleWalker<UnderscoreFieldWalker>
{
    private UnderscoreFieldWalker()
    {
    }

    /// <summary>
    /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
    /// </summary>
    /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
    /// <returns>The <see cref="CodeStyleResult"/>.</returns>
    public static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
    {
        using var walker = Borrow(() => new UnderscoreFieldWalker());
        return await walker.CheckCoreAsync(containing, cancellationToken)
                           .ConfigureAwait(false);
    }

    /// <summary>
    /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
    /// </summary>
    /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
    /// <param name="compilation">The current <see cref="Compilation"/>.</param>
    /// <returns>The <see cref="CodeStyleResult"/>.</returns>
    public static CodeStyleResult Check(SyntaxTree containing, Compilation compilation)
    {
        using var walker = Borrow(() => new UnderscoreFieldWalker());
        return walker.CheckCore(containing, compilation);
    }

    /// <inheritdoc />
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (!node.Modifiers.Any(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.PublicKeyword) &&
            !node.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword))
        {
            foreach (var variable in node.Declaration.Variables)
            {
                var name = variable.Identifier.ValueText;
                this.Update(name.StartsWith("_", StringComparison.Ordinal) ? CodeStyleResult.Yes : CodeStyleResult.No);
            }
        }
    }

    /// <inheritdoc />
    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        // Don't walk, optimization.
    }

    /// <inheritdoc />
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        // Don't walk, optimization.
    }

    /// <inheritdoc />
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // Don't walk, optimization.
    }
}

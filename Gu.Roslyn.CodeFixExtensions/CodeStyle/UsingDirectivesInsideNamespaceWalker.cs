namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Figures out if using directives are placed inside or outside namespaces.
    /// </summary>
    public sealed class UsingDirectivesInsideNamespaceWalker : CompilationStyleWalker<UsingDirectivesInsideNamespaceWalker>
    {
        /// <summary>
        /// Check the <paramref name="containing"/> first. Then check all documents in containing.Project.Documents.
        /// </summary>
        /// <param name="containing">The <see cref="Document"/> containing the currently fixed <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>The <see cref="CodeStyleResult"/>.</returns>
        public static async Task<CodeStyleResult> CheckAsync(Document containing, CancellationToken cancellationToken)
        {
            using var walker = Borrow(() => new UsingDirectivesInsideNamespaceWalker());
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
            using var walker = Borrow(() => new UsingDirectivesInsideNamespaceWalker());
            return walker.CheckCore(containing, compilation);
        }

        /// <inheritdoc />
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            this.Update(node.TryFirstAncestor<NamespaceDeclarationSyntax>(out _) ? CodeStyleResult.Yes : CodeStyleResult.No);
        }

        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Stop walking here
        }

        /// <inheritdoc />
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            // Stop walking here
        }

        /// <inheritdoc />
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // Stop walking here
        }
    }
}

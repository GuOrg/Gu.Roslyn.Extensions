namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="UsingStatementSyntax"/> in the scope.
    /// </summary>
    public sealed class UsingStaticWalker : PooledWalker<UsingStaticWalker>
    {
        private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();

        private UsingStaticWalker()
        {
        }

        /// <summary>
        /// Gets a collection with the <see cref="UsingDirectiveSyntax"/> found when walking.
        /// </summary>
        public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives => this.usingDirectives;

        //public static bool TryGet(SyntaxTree tree, ITypeSymbol type, out UsingDirectiveSyntax result)
        //{
        //    result = null;
        //    if (tree == null ||
        //        type == null)
        //    {
        //        return false;
        //    }

        //    if (tree.TryGetRoot(out var root))
        //    {
        //        using (var walker = Borrow(root))
        //        {
        //            foreach (var candidate in walker.usingDirectives)
        //            {
        //                if (candidate.Alias.)
        //                {
        //                    result = candidate;
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// Get a walker that has visited <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree">The scope.</param>
        /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
        public static UsingStaticWalker Borrow(SyntaxTree tree)
        {
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
            if (tree == null)
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

        /// <inheritdoc />
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node?.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) == true)
            {
                this.usingDirectives.Add(node);
            }

            base.VisitUsingDirective(node);
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

        /// <inheritdoc/>
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // Stop walking here
        }

        /// <inheritdoc/>
        protected override void Clear()
        {
            this.usingDirectives.Clear();
        }
    }
}

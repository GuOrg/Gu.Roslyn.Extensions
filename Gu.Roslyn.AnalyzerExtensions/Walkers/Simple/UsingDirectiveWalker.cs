namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find all <see cref="UsingDirectiveSyntax"/> in the scope.
    /// </summary>
    public sealed class UsingDirectiveWalker : AbstractUsingDirectiveWalker<UsingDirectiveWalker>
    {
        private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();

        /// <summary>
        /// Gets a collection with the <see cref="UsingDirectiveSyntax"/> found when walking.
        /// </summary>
        public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives => this.usingDirectives;

        /// <summary>
        /// Get a walker that has visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The scope.</param>
        /// <returns>A walker that has visited <paramref name="node"/>.</returns>
        public static UsingDirectiveWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new UsingDirectiveWalker());

        /// <summary>
        /// Get a walker that has visited <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree">The scope.</param>
        /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
        public static UsingDirectiveWalker Borrow(SyntaxTree tree)
        {
            if (tree is null)
            {
                throw new System.ArgumentNullException(nameof(tree));
            }

            if (tree.TryGetRoot(out var root))
            {
                return BorrowAndVisit(root, () => new UsingDirectiveWalker());
            }

            return Borrow(() => new UsingDirectiveWalker());
        }

        /// <summary>
        /// Get a walker that has visited <paramref name="tree"/>.
        /// </summary>
        /// <param name="tree">The scope.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A walker that has visited <paramref name="tree"/>.</returns>
        public static async Task<UsingDirectiveWalker> BorrowAsync(SyntaxTree tree, CancellationToken cancellationToken)
        {
            if (tree is null)
            {
                throw new System.ArgumentNullException(nameof(tree));
            }

            var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            return BorrowAndVisit(root, () => new UsingDirectiveWalker());
        }

        /// <inheritdoc />
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            this.usingDirectives.Add(node);
            base.VisitUsingDirective(node);
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.usingDirectives.Clear();
        }
    }
}

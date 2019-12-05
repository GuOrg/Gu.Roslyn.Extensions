namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Find usages of a symbol.
    /// </summary>
    public sealed class UsagesWalker : PooledWalker<UsagesWalker>
    {
        private readonly List<IdentifierNameSyntax> usages = new List<IdentifierNameSyntax>();
        private ISymbol symbol = null!;
        private SemanticModel semanticModel = null!;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Gets a collection with all usages of the symbol.
        /// </summary>
        public IReadOnlyList<IdentifierNameSyntax> Usages => this.usages;

        /// <summary>
        /// Get all usages of <paramref name="localOrParameter"/>.
        /// </summary>
        /// <param name="localOrParameter">The <see cref="LocalOrParameter"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A <see cref="UsagesWalker"/> for <paramref name="localOrParameter"/>.</returns>
        public static UsagesWalker Borrow(LocalOrParameter localOrParameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (localOrParameter.TryGetScope(cancellationToken, out var scope))
            {
                return Borrow(localOrParameter.Symbol, scope, semanticModel, cancellationToken);
            }

            return Borrow(() => new UsagesWalker());
        }

        /// <summary>
        /// Get all usages of <paramref name="fieldOrProperty"/>.
        /// </summary>
        /// <param name="fieldOrProperty">The <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A <see cref="UsagesWalker"/> for <paramref name="fieldOrProperty"/>.</returns>
        public static UsagesWalker Borrow(FieldOrPropertyAndDeclaration fieldOrProperty, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (fieldOrProperty.Declaration.Parent is TypeDeclarationSyntax containingType)
            {
                return Borrow(fieldOrProperty.FieldOrProperty.Symbol, containingType, semanticModel, cancellationToken);
            }

            return Borrow(() => new UsagesWalker());
        }

        /// <summary>
        /// Get all usages of <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="scope">The node to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>A <see cref="UsagesWalker"/> for <paramref name="symbol"/>.</returns>
        public static UsagesWalker Borrow(ISymbol symbol, SyntaxNode scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var walker = Borrow(() => new UsagesWalker());
            walker.symbol = symbol;
            walker.semanticModel = semanticModel;
            walker.cancellationToken = cancellationToken;
            walker.Visit(scope);
            return walker;
        }

        /// <inheritdoc />
        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (IsMatch())
            {
                this.usages.Add(node);
            }

            bool IsMatch()
            {
                return node != null &&
                       NameMatches() &&
                       this.semanticModel.TryGetSymbol(node, this.cancellationToken, out var nodeSymbol) &&
                       nodeSymbol.IsEquivalentTo(this.symbol);

                bool NameMatches()
                {
                    if (string.IsNullOrEmpty(node.Identifier.ValueText))
                    {
                        return false;
                    }

                    if (node.Identifier.ValueText[0] == '@')
                    {
                        return node.Identifier.ValueText.IsParts("@", this.symbol.Name);
                    }

                    return node.Identifier.ValueText == this.symbol.Name;
                }
            }
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            this.usages.Clear();
            this.symbol = null!;
            this.semanticModel = null!;
            this.cancellationToken = CancellationToken.None;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public struct Target<TSymbol, TNode>
        where TSymbol : ISymbol
        where TNode : SyntaxNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target{TSymbol, TNode}"/> struct.
        /// </summary>
        /// <param name="symbol">the symbol.</param>
        /// <param name="node">the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.</param>
        public Target(TSymbol symbol, TNode? node)
        {
            this.Symbol = symbol;
            this.Node = node;
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public TSymbol Symbol { get; }

        /// <summary>
        /// Gets the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.
        /// Null if no declaration was found.
        /// </summary>
        public TNode? Node { get; }
    }
}

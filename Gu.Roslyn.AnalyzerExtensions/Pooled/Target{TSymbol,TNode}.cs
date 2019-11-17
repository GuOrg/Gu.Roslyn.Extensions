namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// This can be for example <see cref="Source"/> = <see cref="ArgumentSyntax"/> <see cref="Symbol"/> = <see cref="IParameterSymbol"/> <see cref="TargetNode"/> = <see cref="BaseMethodDeclarationSyntax"/>.
    /// </summary>
    /// <typeparam name="TSource">The node used in the recursion.</typeparam>
    /// <typeparam name="TSymbol">The target symbol.</typeparam>
    /// <typeparam name="TTarget">The target node.</typeparam>
    public struct Target<TSource, TSymbol, TTarget>
        where TSource : SyntaxNode
        where TSymbol : ISymbol
        where TTarget : SyntaxNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target{TSource,TSymbol, TNode}"/> struct.
        /// </summary>
        /// <param name="source">The <see cref="TSource"/>.</param>
        /// <param name="symbol">the symbol.</param>
        /// <param name="targetNode">the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.</param>
        public Target(TSource source, TSymbol symbol, TTarget? targetNode)
        {
            this.Source = source;
            this.Symbol = symbol;
            this.TargetNode = targetNode;
        }

        public TSource Source { get; }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public TSymbol Symbol { get; }

        /// <summary>
        /// Gets the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.
        /// Null if no declaration was found.
        /// </summary>
        public TTarget? TargetNode { get; }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for creating <see cref="Target{TSource,TSymbol,TTarget}"/>.
    /// </summary>
    public static class Target
    {
        /// <summary>
        /// This can be for example <paramref name="source"/> <see cref="ArgumentSyntax"/> <paramref name="symbol"/> = <see cref="IParameterSymbol"/> <paramref name="target"/> = <see cref="BaseMethodDeclarationSyntax"/>.
        /// Create an instance of <see cref="Target{TSource,TSymbol,TTarget}"/>.
        /// </summary>
        /// <typeparam name="TSource">The node used in the recursion.</typeparam>
        /// <typeparam name="TSymbol">The target symbol.</typeparam>
        /// <typeparam name="TTarget">The target node.</typeparam>
        /// <param name="source">The <see cref="TSource"/>.</param>
        /// <param name="symbol">the symbol.</param>
        /// <param name="targetNode">the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.</param>
        /// <returns>An instance of <see cref="Target{TSource,TSymbol,TTarget}"/>.</returns>
        public static Target<TSource, TSymbol, TNode> Create<TSource, TSymbol, TNode>(TSource source, TSymbol symbol, TNode? target)
            where TSource : SyntaxNode
            where TSymbol : ISymbol
            where TNode : SyntaxNode
        {
            return new Target<TSource, TSymbol, TNode>(source, symbol, target);
        }
    }
}

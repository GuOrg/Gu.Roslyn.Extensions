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
        /// This can be for example <paramref name="source"/> <see cref="ArgumentSyntax"/> <paramref name="symbol"/> = <see cref="IParameterSymbol"/> <paramref name="declaration"/> = <see cref="BaseMethodDeclarationSyntax"/>.
        /// Create an instance of <see cref="Target{TSource,TSymbol,TTarget}"/>.
        /// </summary>
        /// <typeparam name="TSource">The node used in the recursion.</typeparam>
        /// <typeparam name="TSymbol">The target symbol.</typeparam>
        /// <typeparam name="TTarget">The target node.</typeparam>
        /// <param name="source">The <typeparamref name="TSource"/>.</param>
        /// <param name="symbol">the symbol.</param>
        /// <param name="declaration">The declaration for <typeparamref name="TSymbol"/> or the scope if a <see cref="ILocalSymbol"/>.</param>
        /// <returns>An instance of <see cref="Target{TSource,TSymbol,TTarget}"/>.</returns>
        public static Target<TSource, TSymbol, TTarget> Create<TSource, TSymbol, TTarget>(TSource source, TSymbol symbol, TTarget? declaration)
            where TSource : SyntaxNode
            where TSymbol : ISymbol
            where TTarget : SyntaxNode
        {
            return new Target<TSource, TSymbol, TTarget>(source, symbol, declaration);
        }
    }
}

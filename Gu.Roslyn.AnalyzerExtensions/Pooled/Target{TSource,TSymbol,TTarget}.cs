namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// This can be for example <see cref="Source"/> = <see cref="ArgumentSyntax"/> <see cref="Symbol"/> = <see cref="IParameterSymbol"/> <see cref="TargetNode"/> = <see cref="BaseMethodDeclarationSyntax"/>.
    /// </summary>
    /// <typeparam name="TSource">The node used in the recursion.</typeparam>
    /// <typeparam name="TSymbol">The target symbol.</typeparam>
    /// <typeparam name="TTarget">The target node.</typeparam>
    public struct Target<TSource, TSymbol, TTarget> : IEquatable<Target<TSource, TSymbol, TTarget>>
        where TSource : SyntaxNode
        where TSymbol : ISymbol
        where TTarget : SyntaxNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target{TSource,TSymbol, TNode}"/> struct.
        /// </summary>
        /// <param name="source">The <typeparamref name="TSource"/>.</param>
        /// <param name="symbol">the symbol.</param>
        /// <param name="declaration">the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.</param>
        public Target(TSource source, TSymbol symbol, TTarget? declaration)
        {
            this.Source = source;
            this.Symbol = symbol;
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets the source node.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public TSymbol Symbol { get; }

        /// <summary>
        /// Gets the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.
        /// Null if no declaration was found.
        /// </summary>
        public TTarget? Declaration { get; }

        /// <summary>
        /// Gets the declaration for <see cref="Symbol"/> or the scope if a <see cref="ILocalSymbol"/>.
        /// Null if no declaration was found.
        /// </summary>
        [Obsolete("Renamed to Declaration")]
        public TTarget? TargetNode => this.Declaration;

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="Target{TSource, TSymbol, TTarget}"/>.</param>
        /// <param name="right">The right <see cref="Target{TSource, TSymbol, TTarget}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(Target<TSource, TSymbol, TTarget> left, Target<TSource, TSymbol, TTarget> right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="Target{TSource, TSymbol, TTarget}"/>.</param>
        /// <param name="right">The right <see cref="Target{TSource, TSymbol, TTarget}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(Target<TSource, TSymbol, TTarget> left, Target<TSource, TSymbol, TTarget> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(Target<TSource, TSymbol, TTarget> other)
        {
            return EqualityComparer<TSource>.Default.Equals(this.Source, other.Source) &&
                   EqualityComparer<TSymbol>.Default.Equals(this.Symbol, other.Symbol) &&
                   EqualityComparer<TTarget>.Default.Equals(this.Declaration, other.Declaration);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Target<TSource, TSymbol, TTarget> other &&
                   this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<TSource>.Default.GetHashCode(this.Source);
                hashCode = (hashCode * 397) ^ EqualityComparer<TSymbol>.Default.GetHashCode(this.Symbol);
                hashCode = (hashCode * 397) ^ (this.Declaration is null ? 0 : EqualityComparer<TTarget>.Default.GetHashCode(this.Declaration));
                return hashCode;
            }
        }
    }
}

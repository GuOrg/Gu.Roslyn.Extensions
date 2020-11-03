namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A <see cref="ISymbol"/> and its corresponding declaration.
    /// </summary>
    /// <typeparam name="TSymbol">The <see cref="Symbol"/>.</typeparam>
    /// <typeparam name="TDeclaration">The <see cref="MemberDeclarationSyntax"/>.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Nice with fields, not gonna be any side effects.")]
#pragma warning disable RS0016 // Add public types and members to the declared API
    public readonly struct SymbolAndDeclaration<TSymbol, TDeclaration> : IEquatable<SymbolAndDeclaration<TSymbol, TDeclaration>>
#pragma warning restore RS0016 // Add public types and members to the declared API
        where TSymbol : class, ISymbol, IEquatable<ISymbol>
        where TDeclaration : SyntaxNode
    {
        /// <summary>
        /// The <see cref="Symbol"/>.
        /// </summary>
        public readonly TSymbol Symbol;

        /// <summary>
        /// The <see cref="MemberDeclarationSyntax"/>.
        /// </summary>
        public readonly TDeclaration Declaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolAndDeclaration{TSymbol, TDeclaration}"/> struct.
        /// </summary>
        /// <param name="symbol">The <typeparamref name="TSymbol"/>.</param>
        /// <param name="declaration">The <typeparamref name="TDeclaration"/>.</param>
        public SymbolAndDeclaration(TSymbol symbol, TDeclaration declaration)
        {
            this.Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            this.Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
        }

        /// <summary>
        /// Check if <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</param>
        /// <param name="right">The right <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(SymbolAndDeclaration<TSymbol, TDeclaration> left, SymbolAndDeclaration<TSymbol, TDeclaration> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Check if <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</param>
        /// <param name="right">The right <see cref="SymbolAndDeclaration{TSymbol,TDeclaration}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(SymbolAndDeclaration<TSymbol, TDeclaration> left, SymbolAndDeclaration<TSymbol, TDeclaration> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(SymbolAndDeclaration<TSymbol, TDeclaration> other)
        {
            return Equals(this.Symbol, other.Symbol) &&
                   ReferenceEquals(this.Declaration, other.Declaration);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is SymbolAndDeclaration<TSymbol, TDeclaration> other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Symbol.GetHashCode() * 397) ^ this.Declaration.GetHashCode();
            }
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A wrapper for a local or a parameter.
    /// </summary>
    [DebuggerDisplay("{this.Symbol}")]
#pragma warning disable RS0016 // Add public types and members to the declared API
    public readonly struct LocalOrParameter : IEquatable<LocalOrParameter>
#pragma warning restore RS0016 // Add public types and members to the declared API
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalOrParameter"/> struct.
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/>.</param>
        public LocalOrParameter(ILocalSymbol local)
            : this((ISymbol)local)
        {
            if (local is null)
            {
                throw new ArgumentNullException(nameof(local));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalOrParameter"/> struct.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        public LocalOrParameter(IParameterSymbol parameter)
            : this((ISymbol)parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
        }

        private LocalOrParameter(ISymbol symbol)
        {
            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public ISymbol Symbol { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ITypeSymbol Type => (this.Symbol as ILocalSymbol)?.Type ??
                                   ((IParameterSymbol)this.Symbol).Type;

        /// <summary>
        /// Gets the containing symbol.
        /// </summary>
        public ISymbol ContainingSymbol => this.Symbol.ContainingSymbol;

        /// <summary> Gets the symbol name. Returns the empty string if unnamed. </summary>
        public string Name => (this.Symbol as ILocalSymbol)?.Name ?? ((IParameterSymbol)this.Symbol).Name;

        /// <summary>
        /// Check if <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="LocalOrParameter"/>.</param>
        /// <param name="right">The right <see cref="LocalOrParameter"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(LocalOrParameter left, LocalOrParameter right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Check if <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="LocalOrParameter"/>.</param>
        /// <param name="right">The right <see cref="LocalOrParameter"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(LocalOrParameter left, LocalOrParameter right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Try create a <see cref="LocalOrParameter"/> from <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="result">The <see cref="LocalOrParameter"/> if symbol was a local or a parameter.</param>
        /// <returns>True if created a <see cref="LocalOrParameter"/> from <paramref name="symbol"/>.</returns>
        public static bool TryCreate(ISymbol symbol, out LocalOrParameter result)
        {
            switch (symbol)
            {
                case ILocalSymbol local:
                    result = new LocalOrParameter(local);
                    return true;
                case IParameterSymbol parameter:
                    result = new LocalOrParameter(parameter);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        /// <summary>
        /// Try to get the scope where <see cref="Symbol"/> is visible.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>True if a scope could be determined.</returns>
        public bool TryGetScope(CancellationToken cancellationToken, [NotNullWhen(true)] out SyntaxNode? scope)
        {
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return this.Symbol switch
            {
                ILocalSymbol local => local.TryGetScope(cancellationToken, out scope),
                IParameterSymbol parameter => parameter.ContainingSymbol.TrySingleDeclaration(cancellationToken, out scope),
                _ => throw new InvalidOperationException("Should never get here."),
            };
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }

        /// <inheritdoc/>
        public bool Equals(LocalOrParameter other) => this.Symbol switch
        {
            ILocalSymbol local => LocalSymbolComparer.Equal(local, other.Symbol as ILocalSymbol),
            IParameterSymbol parameter => ParameterSymbolComparer.Equal(parameter, other.Symbol as IParameterSymbol),
            _ => false,
        };

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is LocalOrParameter other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Symbol.GetHashCode();
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A wrapper for a local or a parameter.
    /// </summary>
    [DebuggerDisplay("{this.Symbol}")]
    public struct LocalOrParameter
    {
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
        /// Try create a <see cref="LocalOrParameter"/> from <paramref name="symbol"/>
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/></param>
        /// <param name="result">The <see cref="LocalOrParameter"/> if symbol was a local or a parameter.</param>
        /// <returns>True if created a <see cref="LocalOrParameter"/> from <paramref name="symbol"/></returns>
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
                    result = default(LocalOrParameter);
                    return false;
            }
        }
    }
}

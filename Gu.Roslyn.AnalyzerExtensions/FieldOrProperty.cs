namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// A wrapper for a field or a property.
    /// </summary>
    [DebuggerDisplay("{this.Symbol}")]
    public struct FieldOrProperty
    {
        private FieldOrProperty(ISymbol symbol)
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
        public ITypeSymbol Type => (this.Symbol as IFieldSymbol)?.Type ?? ((IPropertySymbol)this.Symbol).Type;

        /// <summary>
        /// Gets the containing type.
        /// </summary>
        public INamedTypeSymbol ContainingType => this.Symbol.ContainingType;

        /// <summary>
        /// Gets the containing type.
        /// </summary>
        public bool IsStatic => this.Symbol.IsStatic;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => (this.Symbol as IFieldSymbol)?.Name ?? ((IPropertySymbol)this.Symbol).Name;

        /// <summary>
        /// Try create a <see cref="FieldOrProperty"/> from <paramref name="symbol"/>
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/></param>
        /// <param name="result">The <see cref="FieldOrProperty"/> if symbol was a field or a property.</param>
        /// <returns>True if created a <see cref="FieldOrProperty"/> from <paramref name="symbol"/></returns>
        public static bool TryCreate(ISymbol symbol, out FieldOrProperty result)
        {
            if (symbol != null)
            {
                if (symbol is IFieldSymbol field)
                {
                    result = new FieldOrProperty(field);
                    return true;
                }

                if (symbol is IPropertySymbol property)
                {
                    result = new FieldOrProperty(property);
                    return true;
                }
            }

            result = default(FieldOrProperty);
            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A wrapper for a field or a property.
    /// </summary>
    [DebuggerDisplay("{this.Symbol}")]
    public struct FieldOrProperty : IEquatable<FieldOrProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldOrProperty"/> struct.
        /// </summary>
        /// <param name="field">The <see cref="IFieldSymbol"/>.</param>
        public FieldOrProperty(IFieldSymbol field)
            : this((ISymbol)field)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldOrProperty"/> struct.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/>.</param>
        public FieldOrProperty(IPropertySymbol property)
            : this((ISymbol)property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }
        }

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
        public ITypeSymbol Type => (this.Symbol as IFieldSymbol)?.Type ??
                                   ((IPropertySymbol)this.Symbol).Type;

        /// <summary>
        /// Gets the containing type.
        /// </summary>
        public INamedTypeSymbol ContainingType => this.Symbol.ContainingType;

        /// <summary>Gets a value indicating whether the symbol is static.</summary>
        public bool IsStatic => this.Symbol.IsStatic;

        /// <summary> Gets the symbol name. Returns the empty string if unnamed. </summary>
        public string Name => (this.Symbol as IFieldSymbol)?.Name ?? ((IPropertySymbol)this.Symbol).Name;

        /// <summary>
        /// Check if <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="FieldOrProperty"/>.</param>
        /// <param name="right">The right <see cref="FieldOrProperty"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(FieldOrProperty left, FieldOrProperty right) => left.Equals(right);

        /// <summary>
        /// Check if <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left <see cref="FieldOrProperty"/>.</param>
        /// <param name="right">The right <see cref="FieldOrProperty"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(FieldOrProperty left, FieldOrProperty right) => !left.Equals(right);

        /// <summary>
        /// Try create a <see cref="FieldOrProperty"/> from <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="result">The <see cref="FieldOrProperty"/> if symbol was a field or a property.</param>
        /// <returns>True if created a <see cref="FieldOrProperty"/> from <paramref name="symbol"/>.</returns>
        public static bool TryCreate(ISymbol symbol, out FieldOrProperty result)
        {
            switch (symbol)
            {
                case IFieldSymbol field:
                    result = new FieldOrProperty(field);
                    return true;
                case IPropertySymbol property:
                    result = new FieldOrProperty(property);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        /// <summary>
        /// Get the initializer or null.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The initializer for the member.</returns>
        public EqualsValueClauseSyntax Initializer(CancellationToken cancellationToken)
        {
            switch (this.Symbol.Kind)
            {
                case SymbolKind.Field
                    when this.Symbol.TrySingleDeclaration(cancellationToken, out FieldDeclarationSyntax? fieldDeclaration) &&
                         fieldDeclaration.Declaration is { Variables: { Count: 1 } variables } &&
                         variables.TrySingle(out var variable) &&
                         variable.Initializer is { } initializer:
                    return initializer;
                case SymbolKind.Property
                    when this.Symbol.TrySingleDeclaration(cancellationToken, out PropertyDeclarationSyntax? propertyDeclaration):
                    return propertyDeclaration.Initializer;
                default:
                    throw new InvalidOperationException("Should never get here.");
            }
        }

        /// <inheritdoc/>
        public bool Equals(FieldOrProperty other) => this.Symbol.Equals(other.Symbol);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FieldOrProperty other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => this.Symbol.GetHashCode();
    }
}

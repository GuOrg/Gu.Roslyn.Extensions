namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A field or a property symbol and declaration.
    /// </summary>
#pragma warning disable RS0016 // Add public types and members to the declared API
    public readonly struct FieldOrPropertyAndDeclaration : IEquatable<FieldOrPropertyAndDeclaration>
#pragma warning restore RS0016 // Add public types and members to the declared API
    {
        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public readonly FieldOrProperty FieldOrProperty;

        /// <summary>
        /// Gets the declaration.
        /// </summary>
        public readonly MemberDeclarationSyntax Declaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldOrPropertyAndDeclaration"/> struct.
        /// </summary>
        /// <param name="field">The <see cref="IFieldSymbol"/>.</param>
        /// <param name="declaration">The <see cref="FieldDeclarationSyntax"/>.</param>
        public FieldOrPropertyAndDeclaration(IFieldSymbol field, FieldDeclarationSyntax declaration)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            this.FieldOrProperty = new FieldOrProperty(field);
            this.Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldOrPropertyAndDeclaration"/> struct.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/>.</param>
        /// <param name="declaration">The <see cref="PropertyDeclarationSyntax"/>.</param>
        public FieldOrPropertyAndDeclaration(IPropertySymbol property, PropertyDeclarationSyntax declaration)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            this.FieldOrProperty = new FieldOrProperty(property);
            this.Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
        }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <param name="right">The right <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(FieldOrPropertyAndDeclaration left, FieldOrPropertyAndDeclaration right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <param name="right">The right <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(FieldOrPropertyAndDeclaration left, FieldOrPropertyAndDeclaration right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Try create a <see cref="FieldOrPropertyAndDeclaration"/>.
        /// </summary>
        /// <param name="memberSymbol">The field or property symbol.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="fieldOrProperty">The <see cref="FieldOrPropertyAndDeclaration"/>.</param>
        /// <returns>True if the symbol is a field or property and a single declaration was found.</returns>
        public static bool TryCreate(ISymbol memberSymbol, CancellationToken cancellationToken, out FieldOrPropertyAndDeclaration fieldOrProperty)
        {
            switch (memberSymbol)
            {
                case IFieldSymbol field
                    when field.TrySingleDeclaration(cancellationToken, out var declaration):
                    fieldOrProperty = new FieldOrPropertyAndDeclaration(field, declaration);
                    return true;
                case IPropertySymbol property
                    when property.TrySingleDeclaration(cancellationToken, out PropertyDeclarationSyntax? declaration):
                    fieldOrProperty = new FieldOrPropertyAndDeclaration(property, declaration);
                    return true;
                default:
                    fieldOrProperty = default;
                    return false;
            }
        }

        /// <inheritdoc/>
        public bool Equals(FieldOrPropertyAndDeclaration other)
        {
            return this.FieldOrProperty.Equals(other.FieldOrProperty);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is FieldOrPropertyAndDeclaration other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.FieldOrProperty.GetHashCode();
        }
    }
}

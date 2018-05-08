// ReSharper disable InconsistentNaming
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="IPropertySymbol"/>
    /// </summary>
    public static class IPropertySymbolExt
    {
        /// <summary>
        /// Get the declaration of the GetMethod if any.
        /// Can be the expression body or the get accessor.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if a declaration was found.</returns>
        public static bool TryGetGetMethodDeclaration(this IPropertySymbol property, CancellationToken cancellationToken, out SyntaxNode declaration)
        {
            declaration = null;
            return property?.GetMethod is IMethodSymbol getMethod &&
                   getMethod.TrySingleDeclaration(cancellationToken, out declaration);
        }

        /// <summary>
        /// Get the get accessor if any.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="getter">The declaration</param>
        /// <returns>True if a declaration was found.</returns>
        public static bool TryGetGetter(this IPropertySymbol property, CancellationToken cancellationToken, out AccessorDeclarationSyntax getter)
        {
            getter = null;
            return property?.GetMethod != null &&
                   property.TrySingleDeclaration(cancellationToken, out var declaration) &&
                   declaration.TryGetGetter(out getter);
        }

        /// <summary>
        /// Get the set accessor if any.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="setter">The declaration</param>
        /// <returns>True if a declaration was found.</returns>
        public static bool TryGetSetter(this IPropertySymbol property, CancellationToken cancellationToken, out AccessorDeclarationSyntax setter)
        {
            setter = null;
            return property?.SetMethod != null &&
                   property.TrySingleDeclaration(cancellationToken, out var declaration) &&
                   declaration.TryGetSetter(out setter);
        }

        /// <summary>
        /// Check if the property is an auto property with get only.
        /// public int Value { get; }
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <returns>True if the property is an auto property with get only.</returns>
        public static bool IsGetOnly(this IPropertySymbol property)
        {
            if (property == null)
            {
                return false;
            }

            return property.SetMethod == null &&
                   property.IsAutoProperty();
        }

        /// <summary>
        /// Check if the property is an auto property.
        /// public int Value { get; private set; }
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/></param>
        /// <returns>True if the property is an auto property.</returns>
        public static bool IsAutoProperty(this IPropertySymbol property)
        {
            if (property?.ContainingType is INamedTypeSymbol containingType)
            {
                foreach (var member in containingType.GetMembers())
                {
                    if (member is IFieldSymbol field &&
                        field.AssociatedSymbol is ISymbol associatedSymbol &&
                        associatedSymbol.Equals(property))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

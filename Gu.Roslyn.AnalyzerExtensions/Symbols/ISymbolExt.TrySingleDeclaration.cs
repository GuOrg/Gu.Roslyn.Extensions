namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for finding the declaration.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ISymbolExt
    {
        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="field">The <see cref="IPropertySymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IFieldSymbol field, CancellationToken cancellationToken, [NotNullWhen(true)] out FieldDeclarationSyntax? declaration)
        {
            if (field is null)
            {
                throw new System.ArgumentNullException(nameof(field));
            }

            declaration = null;
            return field.DeclaringSyntaxReferences.TrySingle(out var reference) &&
                   reference.GetSyntax(cancellationToken)
                            .TryFirstAncestorOrSelf(out declaration);
        }

        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IPropertySymbol property, CancellationToken cancellationToken, [NotNullWhen(true)] out BasePropertyDeclarationSyntax? declaration)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            declaration = property.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as BasePropertyDeclarationSyntax
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="event">The <see cref="IEventSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IEventSymbol @event, CancellationToken cancellationToken, [NotNullWhen(true)] out MemberDeclarationSyntax? declaration)
        {
            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            declaration = @event.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as MemberDeclarationSyntax
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of an event.
        /// </summary>
        /// <param name="event">The <see cref="IEventSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleEventDeclaration(this IEventSymbol @event, CancellationToken cancellationToken, [NotNullWhen(true)] out EventDeclarationSyntax? declaration)
        {
            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            declaration = @event.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as EventDeclarationSyntax
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of an event.
        /// </summary>
        /// <param name="event">The <see cref="IEventSymbol"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleEventFieldDeclaration(this IEventSymbol @event, CancellationToken cancellationToken, [NotNullWhen(true)] out EventFieldDeclarationSyntax? declaration)
        {
            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            declaration = @event.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as EventFieldDeclarationSyntax
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleMethodDeclaration(this IMethodSymbol method, CancellationToken cancellationToken, [NotNullWhen(true)] out MethodDeclarationSyntax? declaration)
        {
            if (method is null)
            {
                throw new System.ArgumentNullException(nameof(method));
            }

            return TrySingleDeclaration(method, cancellationToken, out declaration);
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleAccessorDeclaration(this IMethodSymbol method, CancellationToken cancellationToken, [NotNullWhen(true)] out AccessorDeclarationSyntax? declaration)
        {
            if (method is null)
            {
                throw new System.ArgumentNullException(nameof(method));
            }

            return TrySingleDeclaration(method, cancellationToken, out declaration);
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <typeparam name="T">Either BaseMethodDeclaration or AccessorDeclaration.</typeparam>
        /// <param name="method">The <see cref="IMethodSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration<T>(this IMethodSymbol method, CancellationToken cancellationToken, [NotNullWhen(true)] out T? declaration)
            where T : SyntaxNode
        {
            if (method is null)
            {
                throw new System.ArgumentNullException(nameof(method));
            }

            declaration = method.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as T
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IParameterSymbol parameter, CancellationToken cancellationToken, [NotNullWhen(true)] out ParameterSyntax? declaration)
        {
            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            declaration = parameter.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken) as ParameterSyntax
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of a local.
        /// A local can either be declared using localdeclaration or inline out.
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this ILocalSymbol local, CancellationToken cancellationToken, [NotNullWhen(true)] out SyntaxNode? declaration)
        {
            if (local is null)
            {
                throw new System.ArgumentNullException(nameof(local));
            }

            declaration = local.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken)
                : null;

            return declaration is { };
        }

        /// <summary>
        /// Try to get the single declaration of a local.
        /// A local can either be declared using localdeclaration or inline out.
        /// </summary>
        /// <typeparam name="T">The expected node type.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/>. </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration<T>(this ISymbol symbol, CancellationToken cancellationToken, [NotNullWhen(true)] out T? declaration)
            where T : SyntaxNode
        {
            if (symbol is null)
            {
                throw new System.ArgumentNullException(nameof(symbol));
            }

            declaration = symbol.DeclaringSyntaxReferences.TrySingle(out var reference)
                ? reference.GetSyntax(cancellationToken).FirstAncestorOrSelf<T>()
                : null;

            return declaration is { };
        }
    }
}

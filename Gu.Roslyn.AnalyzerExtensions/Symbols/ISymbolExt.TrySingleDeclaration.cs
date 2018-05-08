namespace Gu.Roslyn.AnalyzerExtensions
{
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
        /// <param name="field">The <see cref="IPropertySymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IFieldSymbol field, CancellationToken cancellationToken, out FieldDeclarationSyntax declaration)
        {
            declaration = null;
            if (field != null &&
                field.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken).FirstAncestorOrSelf<FieldDeclarationSyntax>();
            }

            return declaration != null;
        }

        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="property">The <see cref="IPropertySymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IPropertySymbol property, CancellationToken cancellationToken, out BasePropertyDeclarationSyntax declaration)
        {
            declaration = null;
            if (property != null &&
                property.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken) as BasePropertyDeclarationSyntax;
            }

            return declaration != null;
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleMethodDeclaration(this IMethodSymbol method, CancellationToken cancellationToken, out MethodDeclarationSyntax declaration)
        {
            return TrySingleDeclaration(method, cancellationToken, out declaration);
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <param name="method">The <see cref="IMethodSymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleAccessorDeclaration(this IMethodSymbol method, CancellationToken cancellationToken, out AccessorDeclarationSyntax declaration)
        {
            return TrySingleDeclaration(method, cancellationToken, out declaration);
        }

        /// <summary>
        /// Try to get the single declaration of a method.
        /// </summary>
        /// <typeparam name="T">Either BaseMethodDeclaration or AccessorDeclaration</typeparam>
        /// <param name="method">The <see cref="IMethodSymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration<T>(this IMethodSymbol method, CancellationToken cancellationToken, out T declaration)
            where T : SyntaxNode
        {
            declaration = null;
            if (method != null &&
                method.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken) as T;
            }

            return declaration != null;
        }

        /// <summary>
        /// Try to get the single declaration of a property.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this IParameterSymbol parameter, CancellationToken cancellationToken, out ParameterSyntax declaration)
        {
            declaration = null;
            if (parameter != null &&
                parameter.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken) as ParameterSyntax;
            }

            return declaration != null;
        }

        /// <summary>
        /// Try to get the single declaration of a local.
        /// A local can either be declared using localdeclaration or inline out.
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration(this ILocalSymbol local, CancellationToken cancellationToken, out SyntaxNode declaration)
        {
            declaration = null;
            if (local != null &&
                local.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken);
            }

            return declaration != null;
        }

        /// <summary>
        /// Try to get the single declaration of a local.
        /// A local can either be declared using localdeclaration or inline out.
        /// </summary>
        /// <typeparam name="T">The expected node type.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The declaration</param>
        /// <returns>True if one declaration was found.</returns>
        public static bool TrySingleDeclaration<T>(this ISymbol symbol, CancellationToken cancellationToken, out T declaration)
            where T : SyntaxNode
        {
            declaration = null;
            if (symbol == null)
            {
                return false;
            }

            if (symbol.DeclaringSyntaxReferences.TrySingle(out var reference))
            {
                declaration = reference.GetSyntax(cancellationToken).FirstAncestorOrSelf<T>();
                return declaration != null;
            }

            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension method for <see cref="ConstructorInitializerSyntax"/>.
    /// </summary>
    public static class ConstructorInitializerSyntaxExt
    {
        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="initializer">The <see cref="ConstructorInitializerSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindArgument(this ConstructorInitializerSyntax initializer, IParameterSymbol parameter, [NotNullWhen(true)] out ArgumentSyntax? argument)
        {
            if (initializer is null)
            {
                throw new System.ArgumentNullException(nameof(initializer));
            }

            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            argument = null;
            return initializer?.ArgumentList is { } argumentList &&
                   argumentList.TryFind(parameter, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="initializer">The <see cref="ConstructorInitializerSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="arguments">The <see cref="ImmutableArray{ArgumentSyntax}"/>.</param>
        /// <returns>True if one or more were found.</returns>
        public static bool TryFindArgumentParams(this ConstructorInitializerSyntax initializer, IParameterSymbol parameter, out ImmutableArray<ArgumentSyntax> arguments)
        {
            if (initializer is null)
            {
                throw new System.ArgumentNullException(nameof(initializer));
            }

            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            arguments = default;
            return initializer?.ArgumentList is { } argumentList &&
                   argumentList.TryFindParams(parameter, out arguments);
        }

        /// <summary>
        /// Try getting the declaration of the invoked method.
        /// </summary>
        /// <param name="invocation">The <see cref="ConstructorInitializerSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The <see cref="ConstructorDeclarationSyntax"/>.</param>
        /// <returns>True if the declaration was found.</returns>
        public static bool TryGetTargetDeclaration(this ConstructorInitializerSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out ConstructorDeclarationSyntax? declaration)
        {
            if (invocation is null)
            {
                throw new System.ArgumentNullException(nameof(invocation));
            }

            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            declaration = null;
            return semanticModel.TryGetSymbol(invocation, cancellationToken, out var symbol) &&
                   symbol.TrySingleDeclaration(cancellationToken, out declaration);
        }
    }
}

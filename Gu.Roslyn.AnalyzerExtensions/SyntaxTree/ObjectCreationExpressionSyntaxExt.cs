namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="ObjectCreationExpressionSyntax"/>
    /// </summary>
    public static class ObjectCreationExpressionSyntaxExt
    {
        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="objectCreation">The <see cref="ObjectCreationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindArgument(this ObjectCreationExpressionSyntax objectCreation, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            argument = null;
            return objectCreation?.ArgumentList is ArgumentListSyntax argumentList &&
                  argumentList.TryFind(parameter, out argument);
        }

        /// <summary>
        /// Try getting the declaration of the invoked method.
        /// </summary>
        /// <param name="invocation">The <see cref="ObjectCreationExpressionSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The <see cref="ConstructorDeclarationSyntax"/></param>
        /// <returns>True if the declaration was found.</returns>
        public static bool TryGetTargetDeclaration(this ObjectCreationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out ConstructorDeclarationSyntax declaration)
        {
            declaration = null;
            return semanticModel.TryGetSymbol(invocation, cancellationToken, out var symbol) &&
                   symbol.TrySingleDeclaration(cancellationToken, out declaration);
        }
    }
}

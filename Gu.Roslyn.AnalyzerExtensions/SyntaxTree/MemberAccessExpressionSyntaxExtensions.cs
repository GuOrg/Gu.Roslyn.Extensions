namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension method for working with <see cref="MemberAccessExpressionSyntax"/>.
    /// </summary>
    public static class MemberAccessExpressionSyntaxExtensions
    {
        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="expected"/>.
        /// </summary>
        /// <param name="candidate">The <see cref="MemberAccessExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedField"/> to match against.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target if match.</param>
        /// <returns>True if <paramref name="candidate"/> matches <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this MemberAccessExpressionSyntax candidate, QualifiedField expected, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IFieldSymbol? target)
        {
            target = null;
            return candidate?.Name is IdentifierNameSyntax identifierName &&
                   identifierName.TryGetTarget(expected, semanticModel, cancellationToken, out target);
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="expected"/>.
        /// </summary>
        /// <param name="candidate">The <see cref="MemberAccessExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedProperty"/> to match against.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target if match.</param>
        /// <returns>True if <paramref name="candidate"/> matches <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this MemberAccessExpressionSyntax candidate, QualifiedProperty expected, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IPropertySymbol? target)
        {
            target = null;
            return candidate?.Name is IdentifierNameSyntax identifierName &&
                   identifierName.TryGetTarget(expected, semanticModel, cancellationToken, out target);
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="MemberAccessExpressionSyntax"/>.</param>
        /// <param name="symbol">The <see cref="QualifiedField"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this MemberAccessExpressionSyntax candidate, QualifiedField symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new System.ArgumentNullException(nameof(symbol));
            }

            return candidate switch
            {
                { Name: IdentifierNameSyntax identifier } => identifier.IsSymbol(symbol, semanticModel, cancellationToken),
                { Name.Identifier.ValueText: { } valueText } => valueText == symbol.Name &&
                                                                          semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                                                                          candidateSymbol == symbol,
                _ => semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                     candidateSymbol == symbol,
            };
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="MemberAccessExpressionSyntax"/>.</param>
        /// <param name="symbol">The <see cref="QualifiedProperty"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this MemberAccessExpressionSyntax candidate, QualifiedProperty symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new System.ArgumentNullException(nameof(symbol));
            }

            return candidate switch
            {
                { Name: IdentifierNameSyntax identifier } => identifier.IsSymbol(symbol, semanticModel, cancellationToken),
                { Name.Identifier.ValueText: { } valueText } => valueText == symbol.Name &&
                                                                          semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                                                                          candidateSymbol == symbol,
                _ => semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                     candidateSymbol == symbol,
            };
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="MemberAccessExpressionSyntax"/>.</param>
        /// <param name="symbol">The <see cref="QualifiedMethod"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this MemberAccessExpressionSyntax candidate, QualifiedMethod symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new System.ArgumentNullException(nameof(symbol));
            }

            return candidate switch
            {
                { Name: IdentifierNameSyntax identifier } => identifier.IsSymbol(symbol, semanticModel, cancellationToken),
                { Name.Identifier.ValueText: { } valueText } => valueText == symbol.Name &&
                                                                          semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                                                                          candidateSymbol == symbol,
                _ => semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                     candidateSymbol == symbol,
            };
        }
    }
}

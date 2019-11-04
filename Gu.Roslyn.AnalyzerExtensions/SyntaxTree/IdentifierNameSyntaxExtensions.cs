namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension method for working with <see cref="IdentifierNameSyntax"/>.
    /// </summary>
    public static class IdentifierNameSyntaxExtensions
    {
        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="expected"/>.
        /// </summary>
        /// <param name="candidate">The <see cref="IdentifierNameSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedField"/> to match against.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target if match.</param>
        /// <returns>True if <paramref name="candidate"/> matches <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this IdentifierNameSyntax candidate, QualifiedField expected, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IFieldSymbol? target)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (expected is null)
            {
                throw new System.ArgumentNullException(nameof(expected));
            }

            target = null;
            return candidate.Identifier.ValueText == expected.Name &&
                   semanticModel.TryGetSymbol(candidate, cancellationToken, out target) &&
                   target == expected;
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="expected"/>.
        /// </summary>
        /// <param name="candidate">The <see cref="IdentifierNameSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedProperty"/> to match against.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target if match.</param>
        /// <returns>True if <paramref name="candidate"/> matches <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this IdentifierNameSyntax candidate, QualifiedProperty expected, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IPropertySymbol? target)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (expected is null)
            {
                throw new System.ArgumentNullException(nameof(expected));
            }

            target = null;
            return candidate.Identifier.ValueText == expected.Name &&
                   semanticModel.TryGetSymbol(candidate, cancellationToken, out target) &&
                   target == expected;
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="IdentifierNameSyntax"/>.</param>
        /// <param name="symbol">The <see cref="IsSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this IdentifierNameSyntax candidate, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new System.ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new System.ArgumentNullException(nameof(symbol));
            }

            return candidate.Identifier.ValueText == symbol.Name &&
                   semanticModel.TryGetSymbol(candidate, cancellationToken, out ISymbol? candidateSymbol) &&
                   candidateSymbol.IsEquivalentTo(symbol);
        }
    }
}

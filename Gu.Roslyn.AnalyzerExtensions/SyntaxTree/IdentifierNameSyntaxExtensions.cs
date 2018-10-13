namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension method for working with <see cref="IdentifierNameSyntax"/>.
    /// </summary>
    public static class IdentifierNameSyntaxExtensions
    {
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
            if (symbol.IsEitherKind(SymbolKind.Local, SymbolKind.Parameter) &&
                candidate.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                return false;
            }

            return candidate != null &&
                   candidate.Identifier.ValueText == symbol.Name &&
                   semanticModel.TryGetSymbol(candidate, cancellationToken, out ISymbol candidateSymbol) &&
                   symbol.Equals(candidateSymbol);
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
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
        public static bool TryGetTarget(this MemberAccessExpressionSyntax candidate, QualifiedField expected, SemanticModel semanticModel, CancellationToken cancellationToken, out IFieldSymbol target)
        {
            target = null;
            return candidate.Name is IdentifierNameSyntax identifierName &&
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
        public static bool TryGetTarget(this MemberAccessExpressionSyntax candidate, QualifiedProperty expected, SemanticModel semanticModel, CancellationToken cancellationToken, out IPropertySymbol target)
        {
            target = null;
            return candidate.Name is IdentifierNameSyntax identifierName &&
                   identifierName.TryGetTarget(expected, semanticModel, cancellationToken, out target);
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for determining if parameters are checked for null.
    /// </summary>
    public static class NullCheck
    {
        /// <summary>
        /// Check if <paramref name="parameter"/> is checked for null.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if <paramref name="parameter"/> is checked for null.</returns>
        public static bool IsChecked(IParameterSymbol parameter, SyntaxNode scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (parameter == null ||
                scope == null)
            {
                return false;
            }

            using (var walker = NullCheckWalker.Borrow(scope))
            {
                return walker.TryGetFirst(parameter, semanticModel, cancellationToken, out _);
            }
        }

        /// <summary>
        /// Check if <paramref name="parameter"/> is checked for null before <paramref name="location"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="location">The location where we want to know if <paramref name="parameter"/> is checked for null.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if <paramref name="parameter"/> is checked for null before <paramref name="location"/>.</returns>
        public static bool IsCheckedBefore(IParameterSymbol parameter, SyntaxNode location, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (parameter == null ||
                location == null)
            {
                return false;
            }

            using (var walker = NullCheckWalker.Borrow(location.FirstAncestorOrSelf<MemberDeclarationSyntax>()))
            {
                return walker.TryGetFirst(parameter, semanticModel, cancellationToken, out var check) &&
                       location.TryFirstAncestorOrSelf(out ExpressionSyntax expression) &&
                       check.IsExecutedBefore(expression) == ExecutedBefore.Yes;
            }
        }

        /// <summary>
        /// Get a <see cref="NullCheckWalker"/> filtered by all null checks for <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="scope">The scope to walk.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="NullCheckWalker"/> filtered by all null checks for <paramref name="parameter"/>.</returns>
        public static NullCheckWalker For(IParameterSymbol parameter, SyntaxNode scope, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var walker = NullCheckWalker.Borrow(scope.FirstAncestorOrSelf<MemberDeclarationSyntax>());
            walker.Filter(parameter, semanticModel, cancellationToken);
            return walker;
        }
    }
}

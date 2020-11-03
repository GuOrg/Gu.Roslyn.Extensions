namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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
            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            using var walker = NullCheckWalker.Borrow(scope);
            return walker.TryGetFirst(parameter, semanticModel, cancellationToken, out _);
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
            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            if (location is null)
            {
                throw new System.ArgumentNullException(nameof(location));
            }

            if (location.FirstAncestorOrSelf<MemberDeclarationSyntax>() is { } member)
            {
                using var walker = NullCheckWalker.Borrow(member);
                return walker.TryGetFirst(parameter, semanticModel, cancellationToken, out var check) &&
                       location.TryFirstAncestorOrSelf(out ExpressionSyntax? expression) &&
                       check.IsExecutedBefore(expression) == ExecutedBefore.Yes;
            }

            return false;
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a null check.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="value">The null checked value.</param>
        /// <returns>True if <paramref name="candidate"/> is a null check.</returns>
        public static bool IsNullCheck(ExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out ExpressionSyntax? value)
        {
            if (Equality.IsEqualsCheck(candidate, semanticModel, cancellationToken, out var left, out var right) &&
                IsNullAndExpression(left, right, out value))
            {
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
                return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
            }

            switch (candidate)
            {
                case IsPatternExpressionSyntax { Expression: { } expression, Pattern: ConstantPatternSyntax { Expression: { } constant } }
                    when constant.IsKind(SyntaxKind.NullLiteralExpression):
                    value = expression;
                    return true;
                case BinaryExpressionSyntax node when node.IsKind(SyntaxKind.CoalesceExpression):
                    value = node.Left;
                    return true;
                default:
                    value = null;
                    return false;
            }

            static bool IsNullAndExpression(ExpressionSyntax x, ExpressionSyntax y, out ExpressionSyntax? result)
            {
                if (x.IsKind(SyntaxKind.NullLiteralExpression) && !y.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    result = y;
                    return true;
                }

                if (!x.IsKind(SyntaxKind.NullLiteralExpression) && y.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    result = x;
                    return true;
                }

                result = null;
                return false;
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
            if (parameter is null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            if (scope is null)
            {
                throw new System.ArgumentNullException(nameof(scope));
            }

            if (scope.FirstAncestorOrSelf<MemberDeclarationSyntax>() is {} member)
            {
                var walker = NullCheckWalker.Borrow(member);
                walker.Filter(parameter, semanticModel, cancellationToken);
                return walker;
            }

            return NullCheckWalker.Borrow(scope);
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class EqualsCheck
    {
        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsEqualsCheck(ExpressionSyntax candidate, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            switch (candidate)
            {
                case InvocationExpressionSyntax invocation when invocation.ArgumentList is ArgumentListSyntax argumentList &&
                                                                argumentList.Arguments.Count == 2 &&
                                                                invocation.TryGetMethodName(out var name) &&
                                                                (name == "Equals" || name == "ReferenceEquals"):
                    left = argumentList.Arguments[0].Expression;
                    right = argumentList.Arguments[1].Expression;
                    return true;
                case BinaryExpressionSyntax node when node.IsEither(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression):
                    left = node.Left;
                    right = node.Right;
                    return true;
                default:
                    left = null;
                    right = null;
                    return true;
            }
        }
    }

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
        /// Check if <paramref name="candidate"/> is a nullcheck.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="value">The nullchecked value.</param>
        /// <returns>True if <paramref name="candidate"/> is a nullcheck.</returns>
        public static bool IsNullCheck(ExpressionSyntax candidate, out ExpressionSyntax value)
        {
            if (EqualsCheck.IsEqualsCheck(candidate, out var left, out var right) &&
                IsNullAndExpression(left, right, out value))
            {
                return true;
            }

            switch (candidate)
            {
                case IsPatternExpressionSyntax node when node.Pattern is ConstantPatternSyntax constantPattern &&
                                                         constantPattern.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    value = node.Expression;
                    return true;
                case BinaryExpressionSyntax node when node.IsKind(SyntaxKind.CoalesceExpression):
                    value = node.Left;
                    return true;
                default:
                    value = null;
                    return false;
            }

            bool IsNullAndExpression(ExpressionSyntax x, ExpressionSyntax y, out ExpressionSyntax result)
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
            if (parameter == null)
            {
                throw new System.ArgumentNullException(nameof(parameter));
            }

            if (scope == null)
            {
                throw new System.ArgumentNullException(nameof(scope));
            }

            var walker = NullCheckWalker.Borrow(scope.FirstAncestorOrSelf<MemberDeclarationSyntax>());
            walker.Filter(parameter, semanticModel, cancellationToken);
            return walker;
        }
    }
}

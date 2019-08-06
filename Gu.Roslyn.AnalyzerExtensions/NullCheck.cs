namespace Gu.Roslyn.AnalyzerExtensions
{
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
            switch (candidate)
            {
                case InvocationExpressionSyntax invocation:
                    if (invocation.ArgumentList is ArgumentListSyntax argumentList &&
                        argumentList.Arguments.Count == 2 &&
                        IsNullAndExpression(argumentList.Arguments[0].Expression, argumentList.Arguments[1].Expression, out value) &&
                        invocation.TryGetMethodName(out var name) &&
                        (name == "Equals" || name == "ReferenceEquals"))
                    {
                        return true;
                    }

                    value = null;
                    return false;

                case IsPatternExpressionSyntax node when node.Pattern is ConstantPatternSyntax constantPattern &&
                                                         constantPattern.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    value = node.Expression;
                    return true;
                case BinaryExpressionSyntax node:
                    switch (node.Kind())
                    {
                        case SyntaxKind.EqualsExpression when IsNullAndExpression(node.Left, node.Right, out value):
                        case SyntaxKind.NotEqualsExpression when IsNullAndExpression(node.Left, node.Right, out value):
                            return true;
                        case SyntaxKind.CoalesceExpression when node.Left is ExpressionSyntax expression:
                            value = expression;
                            return true;
                    }

                    value = null;
                    return false;
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

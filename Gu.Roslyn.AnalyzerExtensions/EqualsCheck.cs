namespace Gu.Roslyn.AnalyzerExtensions
{
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
}
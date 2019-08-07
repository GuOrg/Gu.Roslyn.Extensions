namespace Gu.Roslyn.AnalyzerExtensions
{
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
                case InvocationExpressionSyntax invocation when IsObjectEquals(invocation, out left, out right) ||
                                                                IsObjectReferenceEquals(invocation, out left, out right):
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

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectEquals(InvocationExpressionSyntax candidate, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 2 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals")
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                return true;
            }

            left = null;
            right = null;
            return false;
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectReferenceEquals(InvocationExpressionSyntax candidate, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 2 &&
                candidate.TryGetMethodName(out var name) &&
                name == "ReferenceEquals")
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                return true;
            }

            left = null;
            right = null;
            return false;
        }

        /// <summary>
        /// Check if <paramref name="type"/> has op_Equality defined.
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/>.</param>
        /// <returns>True if <paramref name="type"/> has op_Equality defined.</returns>
        public static bool HasEqualityOperator(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Enum:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_DateTime:
                    return true;
            }

            if (type.TypeKind == TypeKind.Enum ||
                type.IsReferenceType)
            {
                return true;
            }

            foreach (var op in type.GetMembers("op_Equality"))
            {
                var opMethod = op as IMethodSymbol;
                if (opMethod?.Parameters.Length == 2 &&
                    type.Equals(opMethod.Parameters[0].Type) &&
                    type.Equals(opMethod.Parameters[1].Type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for equality.
    /// </summary>
    public static class Equality
    {
        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsEqualsCheck(ExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            switch (candidate)
            {
                case InvocationExpressionSyntax invocation when IsObjectEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                                                                IsObjectReferenceEquals(invocation, semanticModel, cancellationToken, out left, out right):
                    return true;
                case BinaryExpressionSyntax node when IsOperatorEquals(node, out left, out right) ||
                                                      IsOperatorNotEquals(node, out left, out right):
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
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectEquals(InvocationExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 2 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals" &&
                IsCorrectSymbol() != false)
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                return true;
            }

            left = null;
            right = null;
            return false;

            bool? IsCorrectSymbol()
            {
                if (semanticModel != null)
                {
                    return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                           method.IsStatic &&
                           method.Parameters.Length == 2 &&
                           method.Parameters[0].Type == QualifiedType.System.Object &&
                           method.Parameters[1].Type == QualifiedType.System.Object;
                }

                return null;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="instance">The left value.</param>
        /// <param name="other">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsInstanceEquals(InvocationExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax instance, out ExpressionSyntax other)
        {
            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 1 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals" &&
                TryGetInstance(out instance) &&
                IsCorrectSymbol() != false)
            {
                other = argumentList.Arguments[0].Expression;
                return true;
            }

            instance = null;
            other = null;
            return false;

            bool TryGetInstance(out ExpressionSyntax result)
            {
                switch (candidate.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccess:
                        result = memberAccess.Expression;
                        return true;
                    case MemberBindingExpressionSyntax _ when candidate.Parent is ConditionalAccessExpressionSyntax conditionalAccess:
                        result = conditionalAccess.Expression;
                        return true;
                    default:
                        result = null;
                        return false;
                }
            }

            bool? IsCorrectSymbol()
            {
                if (semanticModel != null)
                {
                    return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                           !method.IsStatic;
                }

                return null;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectReferenceEquals(InvocationExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 2 &&
                candidate.TryGetMethodName(out var name) &&
                name == "ReferenceEquals" &&
                IsCorrectSymbol() != false)
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                return true;
            }

            left = null;
            right = null;
            return false;

            bool? IsCorrectSymbol()
            {
                if (semanticModel != null)
                {
                    return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                           method.IsStatic &&
                           method.Parameters.Length == 2 &&
                           method.Parameters[0].Type == QualifiedType.System.Object &&
                           method.Parameters[1].Type == QualifiedType.System.Object;
                }

                return null;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsOperatorEquals(BinaryExpressionSyntax candidate, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate.IsKind(SyntaxKind.EqualsExpression))
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                left = candidate.Left;
#pragma warning restore CA1062 // Validate arguments of public methods
                right = candidate.Right;
                return true;
            }

            left = null;
            right = null;
            return false;
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsOperatorNotEquals(BinaryExpressionSyntax candidate, out ExpressionSyntax left, out ExpressionSyntax right)
        {
            if (candidate.IsKind(SyntaxKind.NotEqualsExpression))
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                left = candidate.Left;
#pragma warning restore CA1062 // Validate arguments of public methods
                right = candidate.Right;
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
            if (type == null)
            {
                return false;
            }

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

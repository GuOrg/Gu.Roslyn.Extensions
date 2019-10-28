namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
        public static bool IsEqualsCheck(ExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            switch (candidate)
            {
                case InvocationExpressionSyntax invocation
                    when IsObjectEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                         IsObjectReferenceEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                         IsNullableEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                         IsRuntimeHelpersEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                         IsStringEquals(invocation, semanticModel, cancellationToken, out left, out right, out _) ||
                         IsInstanceEquals(invocation, semanticModel, cancellationToken, out left, out right) ||
                         (semanticModel != null && IsEqualityComparerEquals(invocation, semanticModel, cancellationToken, out _, out left, out right)):
                    return true;
                case ConditionalAccessExpressionSyntax conditionalAccess
                    when conditionalAccess.WhenNotNull is InvocationExpressionSyntax invocation &&
                         IsInstanceEquals(invocation, semanticModel, cancellationToken, out left, out right):
                    return true;
                case BinaryExpressionSyntax node
                    when IsInstanceEquals(node, semanticModel, cancellationToken, out left, out right) ||
                         IsOperatorEquals(node, out left, out right) ||
                         IsOperatorNotEquals(node, out left, out right):
                    return true;
                default:
                    left = null;
                    right = null;
                    return false;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
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
                switch (candidate.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccess when MemberPath.TryFindLast(memberAccess.Expression, out var last) &&
                                                                        last.ValueText == "RuntimeHelpers":
                        return false;
                }

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
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="instance">The left value.</param>
        /// <param name="other">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsInstanceEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? instance, [NotNullWhen(true)]out ExpressionSyntax? other)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 1 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals" &&
                TryGetInstance(candidate, out instance) &&
                IsCorrectSymbol())
            {
                other = argumentList.Arguments[0].Expression;
                return true;
            }

            instance = null;
            other = null;
            return false;

            bool IsCorrectSymbol()
            {
                if (semanticModel != null)
                {
                    return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                           !method.IsStatic;
                }

                return true;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="ConditionalAccessExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="instance">The left value.</param>
        /// <param name="other">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsInstanceEquals(ConditionalAccessExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? instance, [NotNullWhen(true)]out ExpressionSyntax? other)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            return IsInstanceEquals(candidate.WhenNotNull, semanticModel, cancellationToken, out instance, out other);
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
        public static bool IsInstanceEquals(ExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? instance, [NotNullWhen(true)]out ExpressionSyntax? other)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            switch (candidate)
            {
                case InvocationExpressionSyntax invocation:
                    return IsInstanceEquals(invocation, semanticModel, cancellationToken, out instance, out other);
                case ConditionalAccessExpressionSyntax conditionalAccess:
                    return IsInstanceEquals(conditionalAccess, semanticModel, cancellationToken, out instance, out other);
                case BinaryExpressionSyntax binary
                    when IsOperatorEquals(binary, out var x, out var y) ||
                         IsOperatorNotEquals(binary, out x, out y):
                    instance = null;
                    other = null;
                    return IsLiteralAndExpressionAny(x, y, out _, out var expression) &&
                           IsInstanceEquals(expression, semanticModel, cancellationToken, out instance, out other);
                case BinaryExpressionSyntax binary
                    when binary.IsKind(SyntaxKind.CoalesceExpression) &&
                         binary.Right is LiteralExpressionSyntax:
                    return IsInstanceEquals(binary.Left, semanticModel, cancellationToken, out instance, out other);
                default:
                    instance = null;
                    other = null;
                    return false;
            }

            static bool IsLiteralAndExpressionAny(ExpressionSyntax x, ExpressionSyntax y, out LiteralExpressionSyntax literal, out ExpressionSyntax expression)
            {
                return IsLiteralAndExpression(x, y, out literal, out expression) ||
                       IsLiteralAndExpression(y, x, out literal, out expression);
            }

            static bool IsLiteralAndExpression(ExpressionSyntax x, ExpressionSyntax y, out LiteralExpressionSyntax literal, out ExpressionSyntax expression)
            {
                if (x is LiteralExpressionSyntax xl &&
                    !(y is LiteralExpressionSyntax))
                {
                    literal = xl;
                    expression = y;
                    return true;
                }

                literal = null!;
                expression = null!;
                return false;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsObjectReferenceEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
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
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsRuntimeHelpersEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
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
                           method.ContainingType == QualifiedType.System.Runtime.CompilerServices.RuntimeHelpers &&
                           method.IsStatic &&
                           method.Parameters.Length == 2 &&
                           method.Parameters[0].Type == QualifiedType.System.Object &&
                           method.Parameters[1].Type == QualifiedType.System.Object;
                }

                switch (candidate.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccess when MemberPath.TryFindLast(memberAccess.Expression, out var last) &&
                                                                        last.ValueText == "RuntimeHelpers":
                        return null;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is Nullable.Equals(left, right).
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsNullableEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
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
                           method.ContainingType == QualifiedType.System.Nullable &&
                           method.IsStatic &&
                           method.Parameters.Length == 2 &&
                           method.Parameters[0].Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
                           method.Parameters[1].Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
                }

                switch (candidate.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccess when MemberPath.TryFindLast(memberAccess.Expression, out var last) &&
                                                                        last.ValueText == "Nullable":
                        return null;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is Nullable.Equals(left, right).
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="stringComparison">The <see cref="ExpressionSyntax"/> with the <see cref="StringComparison"/>.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsStringEquals(InvocationExpressionSyntax candidate, SemanticModel? semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right, [NotNullWhen(true)]out ExpressionSyntax? stringComparison)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 3 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals" &&
                IsCorrectSymbol())
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                stringComparison = argumentList.Arguments[2].Expression;
                return true;
            }

            left = null;
            right = null;
            stringComparison = null;
            return false;

            bool IsCorrectSymbol()
            {
                if (semanticModel != null)
                {
                    return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                           method.ContainingType == QualifiedType.System.String &&
                           method.IsStatic &&
                           method.Parameters.Length == 3 &&
                           method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
                           method.Parameters[1].Type.SpecialType == SpecialType.System_String &&
                           method.Parameters[2].Type == QualifiedType.System.StringComparison;
                }

                switch (candidate.Expression)
                {
                    case MemberAccessExpressionSyntax memberAccess when MemberPath.TryFindLast(memberAccess.Expression, out var last) &&
                                                                        string.Equals(last.ValueText, "String", StringComparison.OrdinalIgnoreCase):
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is Nullable.Equals(left, right).
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>. If null only the name is checked.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <param name="comparer">The <see cref="ExpressionSyntax"/> for the <see cref="System.Collections.Generic.EqualityComparer{T}"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsEqualityComparerEquals(InvocationExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ExpressionSyntax? comparer, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (candidate?.ArgumentList is ArgumentListSyntax argumentList &&
                argumentList.Arguments.Count == 2 &&
                candidate.TryGetMethodName(out var name) &&
                name == "Equals" &&
                TryGetInstance(candidate, out comparer) &&
                IsCorrectSymbol())
            {
                left = argumentList.Arguments[0].Expression;
                right = argumentList.Arguments[1].Expression;
                return true;
            }

            comparer = null;
            left = null;
            right = null;
            return false;

            bool IsCorrectSymbol()
            {
                return semanticModel.TryGetSymbol(candidate, cancellationToken, out var method) &&
                       method.ContainingType.Is(QualifiedType.System.Collections.Generic.IEqualityComparerOfT) &&
                       !method.IsStatic &&
                       method.Parameters.Length == 2;
            }
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is a check for equality.
        /// Operators == and !=
        /// Equals, ReferenceEquals.
        /// </summary>
        /// <param name="candidate">The <see cref="BinaryExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsOperatorEquals(BinaryExpressionSyntax candidate, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

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
        /// <param name="candidate">The <see cref="BinaryExpressionSyntax"/>.</param>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>True if <paramref name="candidate"/> is a check for equality.</returns>
        public static bool IsOperatorNotEquals(BinaryExpressionSyntax candidate, [NotNullWhen(true)]out ExpressionSyntax? left, [NotNullWhen(true)]out ExpressionSyntax? right)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

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
            if (type is null)
            {
                return false;
            }

            switch (type.SpecialType)
            {
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

            if (type == QualifiedType.System.NullableOfT)
            {
                return type.TryGetSingleTypeArgument(out var typeArgument) &&
                       HasEqualityOperator(typeArgument);
            }

            if (type.TypeKind == TypeKind.Enum ||
                type.IsReferenceType)
            {
                return true;
            }

            return type.GetMembers("op_Equality").TryFirst(out _);
        }

        /// <summary>
        /// Check if <paramref name="type"/> has op_Equality defined.
        /// </summary>
        /// <param name="type">The <see cref="ITypeSymbol"/>.</param>
        /// <returns>True if <paramref name="type"/> has op_Equality defined.</returns>
        public static bool HasOverridenEqualityOperator(ITypeSymbol type)
        {
            if (type is null)
            {
                return false;
            }

            if (type.IsReferenceType)
            {
                return type.GetMembers("op_Equality").TryFirst(out var @operator) &&
                       type.Equals(@operator.ContainingType);
            }

            return HasEqualityOperator(type);
        }

        /// <summary>
        /// Check if Equals or GetHashCode is overriden.
        /// </summary>
        /// <param name="candidate">The <see cref="TypeDeclarationSyntax"/>.</param>
        /// <returns>True if Equals or GetHashCode is overriden.</returns>
        public static bool IsOverriden(TypeDeclarationSyntax candidate)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            foreach (var member in candidate.Members)
            {
                if (member is MethodDeclarationSyntax method &&
                    method.Modifiers.Any(SyntaxKind.OverrideKeyword) &&
                    method.ParameterList is ParameterListSyntax parameterList)
                {
                    if (parameterList.Parameters.Count == 0 &&
                        method.Identifier.ValueText == nameof(GetHashCode))
                    {
                        return true;
                    }

                    if (parameterList.Parameters.TrySingle(out var parameter) &&
                        parameter.Type == QualifiedType.System.Object &&
                        method.Identifier.ValueText == nameof(Equals))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if Equals or GetHashCode is overriden.
        /// </summary>
        /// <param name="candidate">The <see cref="INamedTypeSymbol"/>.</param>
        /// <returns>True if Equals or GetHashCode is overriden.</returns>
        public static bool IsOverriden(INamedTypeSymbol candidate)
        {
            return candidate.TryFindFirstMethod(nameof(Equals), x => x.Parameters.TrySingle(out var parameter) && parameter.Type == QualifiedType.System.Object && x.IsOverride, out _) ||
                   candidate.TryFindFirstMethod(nameof(GetHashCode), x => x.Parameters.Length == 0 && x.IsOverride, out _);
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is negated.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <returns>True if <paramref name="expression"/> is negated.</returns>
        public static bool IsNegated(ExpressionSyntax expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            switch (expression.Parent)
            {
                case ParenthesizedExpressionSyntax paren:
                    return IsNegated(paren);
                case PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.LogicalNotExpression):
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryGetInstance(InvocationExpressionSyntax invocation, [NotNullWhen(true)]out ExpressionSyntax? result)
        {
            switch (invocation.Expression)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    result = memberAccess.Expression;
                    return true;
                case MemberBindingExpressionSyntax _ when invocation.Parent is ConditionalAccessExpressionSyntax conditionalAccess:
                    result = conditionalAccess.Expression;
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="ArgumentSyntax"/>
    /// </summary>
    public static class ArgumentSyntaxExt
    {
        /// <summary>
        /// Try get the value of the argument if it is a constant string.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="result">The string contents of <paramref name="argument"/></param>
        /// <returns>True if the argument expression was a constant string.</returns>
        public static bool TryGetStringValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out string result)
        {
            return TryGetStringValue(argument.Expression, out result) ||
                   semanticModel.TryGetConstantValue(argument.Expression, cancellationToken, out result);

            bool TryGetStringValue(ExpressionSyntax expression, out string text)
            {
                text = null;
                if (expression == null)
                {
                    return false;
                }

                switch (expression)
                {
                    case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                        text = literal.Token.ValueText;
                        return true;
                    case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.NullLiteralExpression):
                        text = null;
                        return true;
                    case CastExpressionSyntax cast:
                        return TryGetStringValue(cast.Expression, out text);
                    case InvocationExpressionSyntax invocation when invocation.IsNameOf():
                        if (invocation.ArgumentList != null &&
                            invocation.ArgumentList.Arguments.TrySingle(out var nameofArg))
                        {
                            switch (nameofArg.Expression)
                            {
                                case IdentifierNameSyntax identifierName:
                                    text = identifierName.Identifier.ValueText;
                                    return true;
                                case MemberAccessExpressionSyntax memberAccess:
                                    text = memberAccess.Name.Identifier.ValueText;
                                    return true;
                                default:
                                    var constantValue = semanticModel.GetConstantValueSafe(invocation, cancellationToken);
                                    if (constantValue.HasValue &&
                                        constantValue.Value is string s)
                                    {
                                        text = s;
                                        return true;
                                    }

                                    break;
                            }
                        }

                        return false;
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Name.Identifier.ValueText == "Empty":
                        switch (memberAccess.Expression)
                        {
                            case PredefinedTypeSyntax predefinedType when predefinedType.Keyword.ValueText == "string":
                                text = string.Empty;
                                return true;
                            case IdentifierNameSyntax source when string.Equals(source.Identifier.ValueText, "string", StringComparison.OrdinalIgnoreCase):
                                text = string.Empty;
                                return true;
                        }

                        return false;
                }

                return false;
            }
        }

        /// <summary>
        /// Try get the value of the argument if it is a typeof() call.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="result">The string contents of <paramref name="argument"/></param>
        /// <returns>True if the call is typeof() and we could figure out the type.</returns>
        public static bool TryGetTypeofValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out ITypeSymbol result)
        {
            result = null;
            if (argument?.Expression == null || semanticModel == null)
            {
                return false;
            }

            if (argument.Expression is TypeOfExpressionSyntax typeOf)
            {
                var typeSyntax = typeOf.Type;
                var typeInfo = semanticModel.GetTypeInfoSafe(typeSyntax, cancellationToken);
                result = typeInfo.Type;
                return result != null;
            }

            return false;
        }

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="method">The <see cref="BaseMethodDeclarationSyntax"/></param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/></param>
        /// <returns>True if a matching poarameter was found.</returns>
        public static bool TryFindParameter(this ArgumentSyntax argument, BaseMethodDeclarationSyntax method, out ParameterSyntax parameter)
        {
            return method.TryFindParameter(argument, out parameter);
        }

        /// <summary>
        /// Find the matching parameter for the argument.
        /// </summary>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <param name="method">The <see cref="IMethodSymbol"/></param>
        /// <param name="parameter">The matching <see cref="ParameterSyntax"/></param>
        /// <returns>True if a matching poarameter was found.</returns>
        public static bool TryFindParameter(this ArgumentSyntax argument, IMethodSymbol method, out IParameterSymbol parameter)
        {
            return method.TryFindParameter(argument, out parameter);
        }
    }
}

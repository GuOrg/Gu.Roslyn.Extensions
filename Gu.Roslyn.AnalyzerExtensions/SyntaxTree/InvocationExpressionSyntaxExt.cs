namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class InvocationExpressionSyntaxExt
    {
        public static bool IsPotentialReturnVoid(this InvocationExpressionSyntax invocation)
        {
            if (invocation.Parent is ArgumentSyntax ||
                invocation.Parent is EqualsValueClauseSyntax ||
                invocation.Parent is AssignmentExpressionSyntax)
            {
                return false;
            }

            if (invocation.Parent is IfStatementSyntax ifStatement &&
                ifStatement.Condition.Contains(invocation))
            {
                return false;
            }

            return true;
        }

        public static bool TryGetMethodName(this InvocationExpressionSyntax invocation, out string name)
        {
            name = null;
            if (invocation == null)
            {
                return false;
            }

            switch (invocation.Kind())
            {
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.TypeOfExpression:
                    if (invocation.Expression is SimpleNameSyntax simple)
                    {
                        name = simple.Identifier.ValueText;
                        return true;
                    }

                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name is SimpleNameSyntax member)
                    {
                        name = member.Identifier.ValueText;
                        return true;
                    }

                    if (invocation.Expression is MemberBindingExpressionSyntax memberBinding &&
                        memberBinding.Name is SimpleNameSyntax bound)
                    {
                        name = bound.Identifier.ValueText;
                        return true;
                    }

                    return false;
                default:
                    return false;
            }
        }

        internal static bool TryGetInvokedSymbol(this InvocationExpressionSyntax invocation, QualifiedMethod expected, SemanticModel semanticModel, CancellationToken cancellationToken, out IMethodSymbol result)
        {
            result = null;
            if (invocation == null)
            {
                return false;
            }

            if (invocation.TryGetMethodName(out var name) &&
                name != expected.Name)
            {
                return false;
            }

            if (semanticModel.GetSymbolSafe(invocation, cancellationToken) is IMethodSymbol candidate &&
                candidate == expected)
            {
                result = candidate;
                return true;
            }

            return false;
        }

        internal static bool IsNameOf(this InvocationExpressionSyntax invocation)
        {
            return invocation.TryGetMethodName(out var name) &&
                   name == "nameof";
        }
    }
}
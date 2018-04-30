namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for <see cref="InvocationExpressionSyntax"/>
    /// </summary>
    public static class InvocationExpressionSyntaxExt
    {
        /// <summary>
        /// Check if the invocation is potentially returning void from the usage.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <returns>True if it possibly return void.</returns>
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

        /// <summary>
        /// Check if the invocation is potentially is a member call in the containing instance.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <returns>True if it possibly is a member call in the containing instance.</returns>
        public static bool IsPotentialThis(this InvocationExpressionSyntax invocation)
        {
            switch (invocation.Expression)
            {
                case IdentifierNameSyntax _:
                    return true;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the invocation is potentially is a member or base call in the containing instance.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <returns>True if it possibly is a member call in the containing instance.</returns>
        public static bool IsPotentialThisOrBase(this InvocationExpressionSyntax invocation)
        {
            switch (invocation.Expression)
            {
                case IdentifierNameSyntax _:
                    return true;
                case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is InstanceExpressionSyntax:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get the invoked method's name.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <param name="name">The name of the invoked method.</param>
        /// <returns>True if the name was found.</returns>
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

        /// <summary>
        /// Check if the invocation is nameof()
        /// </summary>
        /// <param name="invocation">The invocation</param>
        /// <returns>True if the invocation is nameof()</returns>
        public static bool IsNameOf(this InvocationExpressionSyntax invocation)
        {
            return invocation.TryGetMethodName(out var name) &&
                   name == "nameof";
        }

        /// <summary>
        /// Check if the invocation is typeof()
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <returns>True if the invocation is typeof()</returns>
        public static bool IsTypeOf(this InvocationExpressionSyntax invocation)
        {
            return invocation.TryGetMethodName(out var name) &&
                   name == "typeof";
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/></param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindArgument(this InvocationExpressionSyntax invocation, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            argument = null;
            return invocation?.ArgumentList is ArgumentListSyntax argumentList &&
                   argumentList.TryFind(parameter, out argument);
        }

        /// <summary>
        /// Try getting the declaration of the invoked method.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="declaration">The <see cref="MethodDeclarationSyntax"/></param>
        /// <returns>True if the declaration was found.</returns>
        public static bool TryGetMethodDeclaration(this InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out MethodDeclarationSyntax declaration)
        {
            declaration = null;
            return semanticModel.TryGetSymbol(invocation, cancellationToken, out var symbol) &&
                   symbol.TrySingleDeclaration(cancellationToken, out declaration);
        }
    }
}

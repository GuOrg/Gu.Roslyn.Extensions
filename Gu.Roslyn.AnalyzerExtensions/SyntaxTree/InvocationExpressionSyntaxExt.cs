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

            return MemberPath.IsEmpty(invocation);
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

            return MemberPath.IsEmpty(invocation);
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
        /// Check if <paramref name="invocation"/> is a call to <paramref name="expected"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedMethod"/> to match against.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target method.</param>
        /// <returns>True if <paramref name="invocation"/> is a call to <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, SemanticModel semanticModel, CancellationToken cancellationToken, out IMethodSymbol target)
        {
            target = null;
            return invocation.ArgumentList != null &&
                   invocation.TryGetMethodName(out var name) &&
                   name == expected.Name &&
                   semanticModel.TryGetSymbol(invocation, cancellationToken, out target) &&
                   target == expected;
        }

        /// <summary>
        /// Check if <paramref name="invocation"/> is a call to <paramref name="expected"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedMethod"/> to match against.</param>
        /// <param name="qualifiedParameter0">The expected parameter.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target method.</param>
        /// <param name="argument">The argument if match.</param>
        /// <returns>True if <paramref name="invocation"/> is a call to <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, QualifiedParameter qualifiedParameter0, SemanticModel semanticModel, CancellationToken cancellationToken, out IMethodSymbol target, out ArgumentSyntax argument)
        {
            argument = null;
            return TryGetTarget(invocation, expected, semanticModel, cancellationToken, out target) &&
                   target.Parameters.TrySingle(out var parameter) &&
                   parameter == qualifiedParameter0 &&
                   invocation.TryFindArgument(parameter, out argument);
        }

        /// <summary>
        /// Check if <paramref name="invocation"/> is a call to <paramref name="expected"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedMethod"/> to match against.</param>
        /// <param name="qualifiedParameter0">The first parameter.</param>
        /// <param name="qualifiedParameter1">The second parameter.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target method.</param>
        /// <param name="argument0">The first argument if match.</param>
        /// <param name="argument1">The second argument if match.</param>
        /// <returns>True if <paramref name="invocation"/> is a call to <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, QualifiedParameter qualifiedParameter0, QualifiedParameter qualifiedParameter1, SemanticModel semanticModel, CancellationToken cancellationToken, out IMethodSymbol target, out ArgumentSyntax argument0, out ArgumentSyntax argument1)
        {
            argument0 = null;
            argument1 = null;
            return TryGetTarget(invocation, expected, semanticModel, cancellationToken, out target) &&
                   target.Parameters.Length == 2 &&
                   target.Parameters[0] == qualifiedParameter0 &&
                   target.Parameters[1] == qualifiedParameter1 &&
                   invocation.TryFindArgument(target.Parameters[0], out argument0) &&
                   invocation.TryFindArgument(target.Parameters[1], out argument1);
        }

        /// <summary>
        /// Check if <paramref name="invocation"/> is a call to <paramref name="expected"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedMethod"/> to match against.</param>
        /// <param name="qualifiedParameter0">The first parameter.</param>
        /// <param name="qualifiedParameter1">The second parameter.</param>
        /// <param name="qualifiedParameter2">The third parameter.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target method.</param>
        /// <param name="argument0">The first argument if match.</param>
        /// <param name="argument1">The second argument if match.</param>
        /// <param name="argument2">The third argument if match.</param>
        /// <returns>True if <paramref name="invocation"/> is a call to <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(
            this InvocationExpressionSyntax invocation,
            QualifiedMethod expected,
            QualifiedParameter qualifiedParameter0,
            QualifiedParameter qualifiedParameter1,
            QualifiedParameter qualifiedParameter2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out IMethodSymbol target,
            out ArgumentSyntax argument0,
            out ArgumentSyntax argument1,
            out ArgumentSyntax argument2)
        {
            argument0 = null;
            argument1 = null;
            argument2 = null;
            return TryGetTarget(invocation, expected, semanticModel, cancellationToken, out target) &&
                   target.Parameters.Length == 3 &&
                   target.Parameters[0] == qualifiedParameter0 &&
                   target.Parameters[1] == qualifiedParameter1 &&
                   target.Parameters[2] == qualifiedParameter2 &&
                   invocation.TryFindArgument(target.Parameters[0], out argument0) &&
                   invocation.TryFindArgument(target.Parameters[1], out argument1) &&
                   invocation.TryFindArgument(target.Parameters[2], out argument2);
        }

        /// <summary>
        /// Check if <paramref name="invocation"/> is a call to <paramref name="expected"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="expected">The <see cref="QualifiedMethod"/> to match against.</param>
        /// <param name="qualifiedParameter0">The first parameter.</param>
        /// <param name="qualifiedParameter1">The second parameter.</param>
        /// <param name="qualifiedParameter2">The third parameter.</param>
        /// <param name="qualifiedParameter3">The fourth parameter.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="target">The symbol of the target method.</param>
        /// <param name="argument0">The first argument if match.</param>
        /// <param name="argument1">The second argument if match.</param>
        /// <param name="argument2">The third argument if match.</param>
        /// <param name="argument3">The fourth argument if match.</param>
        /// <returns>True if <paramref name="invocation"/> is a call to <paramref name="expected"/>.</returns>
        public static bool TryGetTarget(
            this InvocationExpressionSyntax invocation,
            QualifiedMethod expected,
            QualifiedParameter qualifiedParameter0,
            QualifiedParameter qualifiedParameter1,
            QualifiedParameter qualifiedParameter2,
            QualifiedParameter qualifiedParameter3,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out IMethodSymbol target,
            out ArgumentSyntax argument0,
            out ArgumentSyntax argument1,
            out ArgumentSyntax argument2,
            out ArgumentSyntax argument3)
        {
            argument0 = null;
            argument1 = null;
            argument2 = null;
            argument3 = null;
            return TryGetTarget(invocation, expected, semanticModel, cancellationToken, out target) &&
                   target.Parameters.Length == 4 &&
                   target.Parameters[0] == qualifiedParameter0 &&
                   target.Parameters[1] == qualifiedParameter1 &&
                   target.Parameters[2] == qualifiedParameter2 &&
                   target.Parameters[3] == qualifiedParameter3 &&
                   invocation.TryFindArgument(target.Parameters[0], out argument0) &&
                   invocation.TryFindArgument(target.Parameters[1], out argument1) &&
                   invocation.TryFindArgument(target.Parameters[2], out argument2) &&
                   invocation.TryFindArgument(target.Parameters[3], out argument3);
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
        public static bool TryGetTargetDeclaration(this InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out MethodDeclarationSyntax declaration)
        {
            declaration = null;
            return semanticModel.TryGetSymbol(invocation, cancellationToken, out var symbol) &&
                   symbol.TrySingleDeclaration(cancellationToken, out declaration);
        }
    }
}

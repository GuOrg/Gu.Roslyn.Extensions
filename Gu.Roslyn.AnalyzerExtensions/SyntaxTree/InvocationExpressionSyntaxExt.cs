namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for <see cref="InvocationExpressionSyntax"/>.
    /// </summary>
    public static class InvocationExpressionSyntaxExt
    {
        /// <summary>
        /// Check if the invocation is potentially returning void from the usage.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <returns>True if it possibly return void.</returns>
        public static bool IsPotentialReturnVoid(this InvocationExpressionSyntax invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return invocation.Parent switch
            {
                ArgumentSyntax _ => false,
                EqualsValueClauseSyntax _ => false,
                AssignmentExpressionSyntax _ => false,
                IfStatementSyntax ifStatement => !ifStatement.Condition.Contains(invocation),
                _ => true,
            };
        }

        /// <summary>
        /// Check if the invocation is potentially is a member call in the containing instance.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <returns>True if it possibly is a member call in the containing instance.</returns>
        public static bool IsPotentialThis(this InvocationExpressionSyntax invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return invocation.Expression switch
            {
                IdentifierNameSyntax _ => true,
                MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax _ } => true,
                _ => MemberPath.IsEmpty(invocation),
            };
        }

        /// <summary>
        /// Check if the invocation is potentially is a member or base call in the containing instance.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <returns>True if it possibly is a member call in the containing instance.</returns>
        public static bool IsPotentialThisOrBase(this InvocationExpressionSyntax invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return invocation.Expression switch
            {
                IdentifierNameSyntax _ => true,
                MemberAccessExpressionSyntax { Expression: InstanceExpressionSyntax _ } => true,
                _ => MemberPath.IsEmpty(invocation),
            };
        }

        /// <summary>
        /// Try to get the invoked method's name.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <returns>The name of the invoked method.</returns>
        public static string? MethodName(this InvocationExpressionSyntax invocation)
        {
            return TryGetMethodName(invocation, out var name) ? name : null;
        }

        /// <summary>
        /// Try to get the invoked method's name.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="name">The name of the invoked method.</param>
        /// <returns>True if the name was found.</returns>
        public static bool TryGetMethodName(this InvocationExpressionSyntax invocation, [NotNullWhen(true)] out string? name)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            switch (invocation.Expression)
            {
                case SimpleNameSyntax { Identifier: { ValueText: { } valueText } }:
                    name = valueText;
                    return true;
                case MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: { } valueText } } }:
                    name = valueText;
                    return true;
                case MemberBindingExpressionSyntax { Name: { Identifier: { ValueText: { } valueText } } }:
                    name = valueText;
                    return true;
                default:
                    name = null;
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
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? target)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            target = null;
            if (invocation is null)
            {
                return false;
            }

            return invocation.ArgumentList is { } &&
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
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, QualifiedParameter qualifiedParameter0, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? target, [NotNullWhen(true)] out ArgumentSyntax? argument)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
        public static bool TryGetTarget(this InvocationExpressionSyntax invocation, QualifiedMethod expected, QualifiedParameter qualifiedParameter0, QualifiedParameter qualifiedParameter1, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IMethodSymbol? target, [NotNullWhen(true)] out ArgumentSyntax? argument0, [NotNullWhen(true)] out ArgumentSyntax? argument1)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
            [NotNullWhen(true)] out IMethodSymbol? target,
            [NotNullWhen(true)] out ArgumentSyntax? argument0,
            [NotNullWhen(true)] out ArgumentSyntax? argument1,
            [NotNullWhen(true)] out ArgumentSyntax? argument2)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
            [NotNullWhen(true)] out IMethodSymbol? target,
            [NotNullWhen(true)] out ArgumentSyntax? argument0,
            [NotNullWhen(true)] out ArgumentSyntax? argument1,
            [NotNullWhen(true)] out ArgumentSyntax? argument2,
            [NotNullWhen(true)] out ArgumentSyntax? argument3)
        {
            if (expected is null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

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
        /// Check if the invocation is nameof().
        /// </summary>
        /// <param name="node">The invocation.</param>
        /// <returns>True if the invocation is nameof().</returns>
        public static bool IsNameOf(this InvocationExpressionSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.TryGetMethodName(out var name) &&
                   name == "nameof";
        }

        /// <summary>
        /// Check if the invocation is typeof().
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <returns>True if the invocation is typeof().</returns>
        public static bool IsTypeOf(this InvocationExpressionSyntax invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return invocation.TryGetMethodName(out var name) &&
                   name == "typeof";
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="node">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <returns><see cref="ArgumentSyntax"/> if a match was found.</returns>
        public static ArgumentSyntax? FindArgument(this InvocationExpressionSyntax node, IParameterSymbol parameter)
        {
            return TryFindArgument(node, parameter, out var match) ? match : null;
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindArgument(this InvocationExpressionSyntax invocation, IParameterSymbol parameter, [NotNullWhen(true)] out ArgumentSyntax? argument)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            argument = null;
            return invocation.ArgumentList is { } argumentList &&
                   argumentList.TryFind(parameter, out argument);
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="arguments">The <see cref="ImmutableArray{ArgumentSyntax}"/>.</param>
        /// <returns>True if one or more were found.</returns>
        public static bool TryFindArgumentParams(this InvocationExpressionSyntax invocation, IParameterSymbol parameter, out ImmutableArray<ArgumentSyntax> arguments)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            arguments = default;
            return invocation.ArgumentList is { } argumentList &&
                   argumentList.TryFindParams(parameter, out arguments);
        }

        /// <summary>
        /// Try getting the declaration of the invoked method.
        /// </summary>
        /// <param name="node">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MethodDeclarationSyntax"/> if the declaration was found.</returns>
        public static MethodDeclarationSyntax? TargetDeclaration(this InvocationExpressionSyntax node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return TryGetTargetDeclaration(node, semanticModel, cancellationToken, out var declaration) ? declaration : null;
        }

        /// <summary>
        /// Try getting the declaration of the invoked method.
        /// </summary>
        /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="declaration">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <returns>True if the declaration was found.</returns>
        public static bool TryGetTargetDeclaration(this InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out MethodDeclarationSyntax? declaration)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            declaration = null;
            return semanticModel.TryGetSymbol(invocation, cancellationToken, out var symbol) &&
                   symbol.TrySingleDeclaration(cancellationToken, out declaration);
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="symbol">The <see cref="ISymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this InvocationExpressionSyntax candidate, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (candidate.TryGetMethodName(out var name) &&
                name != symbol.Name)
            {
                return false;
            }

            return semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                   candidateSymbol.IsEquivalentTo(symbol);
        }

        /// <summary>
        /// Check if <paramref name="candidate"/> is <paramref name="symbol"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="candidate">The <see cref="InvocationExpressionSyntax"/>.</param>
        /// <param name="symbol">The <see cref="QualifiedMethod"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="candidate"/> is <paramref name="symbol"/>.</returns>
        public static bool IsSymbol(this InvocationExpressionSyntax candidate, QualifiedMethod symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (candidate.TryGetMethodName(out var name) &&
                name != symbol.Name)
            {
                return false;
            }

            return semanticModel.TryGetSymbol(candidate, cancellationToken, out var candidateSymbol) &&
                   candidateSymbol == symbol;
        }
    }
}

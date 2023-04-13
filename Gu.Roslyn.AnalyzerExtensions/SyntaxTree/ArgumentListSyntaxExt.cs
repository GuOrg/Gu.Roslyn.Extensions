namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ArgumentListSyntax"/>.
    /// </summary>
    public static class ArgumentListSyntaxExt
    {
        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <returns>The <see cref="ArgumentSyntax"/> if a match was found.</returns>
        public static ArgumentSyntax? Find(this ArgumentListSyntax argumentList, IParameterSymbol parameter)
        {
            return TryFind(argumentList, parameter, out var match) ? match : null;
        }

        /// <summary>
        /// Get the argument that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFind(this ArgumentListSyntax argumentList, IParameterSymbol parameter, [NotNullWhen(true)] out ArgumentSyntax? argument)
        {
            if (argumentList is null)
            {
                throw new ArgumentNullException(nameof(argumentList));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            switch (argumentList)
            {
                case { Arguments.Count: 0 }:
                    argument = null;
                    return false;
                case { Arguments: { } arguments }:
                    if (TryFindByNameColon(argumentList, parameter.Name, out argument))
                    {
                        return true;
                    }

                    if (parameter.IsParams)
                    {
                        argument = null;
                        return arguments.Count - 1 == Index() &&
                               arguments.TryElementAt(Index(), out argument);
                    }

                    if (parameter.IsOptional)
                    {
                        if (argumentList.Arguments.TryElementAt(Index(), out var candidate) &&
                            candidate.NameColon is null)
                        {
                            argument = candidate;
                            return true;
                        }

                        return false;
                    }

                    return argumentList.Arguments.TryElementAt(Index(), out argument);
            }

            argument = null;
            return false;

            int Index()
            {
                return parameter switch
                {
                    { ContainingSymbol: IMethodSymbol { IsExtensionMethod: true, ReducedFrom: null } }
                        when IsExtensionMethodInvocation()
                        => parameter.Ordinal - 1,
                    { ContainingSymbol: IMethodSymbol { IsExtensionMethod: true, ReducedFrom: { } } }
                        when !IsExtensionMethodInvocation()
                        => parameter.Ordinal + 1,
                    _ => parameter.Ordinal,
                };
            }

            bool IsExtensionMethodInvocation()
            {
                return argumentList switch
                {
                    { Parent: InvocationExpressionSyntax { Parent: ConditionalAccessExpressionSyntax } } => true,
                    { Parent: InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess } }
                        => memberAccess switch
                        {
                            { Expression: IdentifierNameSyntax name } => name.Identifier.ValueText != parameter.ContainingType.Name,
                            { Expression: MemberAccessExpressionSyntax { Name: IdentifierNameSyntax name } } => name.Identifier.ValueText != parameter.ContainingType.Name,
                            _ => true,
                        },
                    _ => false,
                };
            }
        }

        /// <summary>
        /// Get the arguments that matches <paramref name="parameter"/>.
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/>.</param>
        /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
        /// <param name="arguments">The <see cref="ImmutableArray{ArgumentSyntax}"/>.</param>
        /// <returns>True if one or more were found.</returns>
        public static bool TryFindParams(this ArgumentListSyntax argumentList, IParameterSymbol parameter, out ImmutableArray<ArgumentSyntax> arguments)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (argumentList is null)
            {
                arguments = default;
                return false;
            }

            if (parameter.IsParams)
            {
                var builder = ImmutableArray.CreateBuilder<ArgumentSyntax>(argumentList.Arguments.Count - parameter.Ordinal);
                for (var i = parameter.Ordinal; i < argumentList.Arguments.Count; i++)
                {
                    builder.Add(argumentList.Arguments[i]);
                }

                arguments = builder.MoveToImmutable();
                return true;
            }

            arguments = default;
            return false;
        }

        /// <summary>
        /// Get the argument that matches <paramref name="name"/>.
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindByNameColon(this ArgumentListSyntax argumentList, string name, [NotNullWhen(true)] out ArgumentSyntax? argument)
        {
            if (argumentList is null)
            {
                throw new ArgumentNullException(nameof(argumentList));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (var candidate in argumentList.Arguments)
            {
                if (candidate.NameColon is { Name.Identifier.ValueText: { } valueText } &&
                    valueText == name)
                {
                    argument = candidate;
                    return true;
                }
            }

            argument = null;
            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
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
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFind(this ArgumentListSyntax argumentList, IParameterSymbol parameter, out ArgumentSyntax argument)
        {
            argument = null;
            if (argumentList is null ||
                argumentList.Arguments.Count == 0 ||
                parameter is null)
            {
                return false;
            }

            if (parameter.IsParams)
            {
                return argumentList.Arguments.Count - 1 == parameter.Ordinal &&
                       argumentList.Arguments.TryElementAt(parameter.Ordinal, out argument);
            }

            if (TryFindByNameColon(argumentList, parameter.Name, out argument))
            {
                return true;
            }

            if (parameter.IsOptional)
            {
                if (argumentList.Arguments.TryElementAt(parameter.Ordinal, out var candidate) &&
                    candidate.NameColon is null)
                {
                    argument = candidate;
                    return true;
                }

                return false;
            }

            return argumentList.Arguments.TryElementAt(parameter.Ordinal, out argument);
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
                throw new System.ArgumentNullException(nameof(parameter));
            }

            if (argumentList is null)
            {
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

            return false;
        }

        /// <summary>
        /// Get the argument that matches <paramref name="name"/>.
        /// </summary>
        /// <param name="argumentList">The <see cref="ArgumentListSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <param name="argument">The <see cref="ArgumentSyntax"/>.</param>
        /// <returns>True if a match was found.</returns>
        public static bool TryFindByNameColon(this ArgumentListSyntax argumentList, string name, out ArgumentSyntax argument)
        {
            argument = null;
            if (argumentList is null)
            {
                return false;
            }

            foreach (var candidate in argumentList.Arguments)
            {
                if (candidate.NameColon is NameColonSyntax nameColon &&
                    nameColon.Name.Identifier.ValueText == name)
                {
                    argument = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}

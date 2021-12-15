namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="ILocalSymbol"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILocalSymbolExt
    {
        /// <summary>
        /// Try to get the scope where <paramref name="local"/> is visible.
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>True if a scope could be determined.</returns>
        public static bool TryGetScope(this ILocalSymbol local, CancellationToken cancellationToken, [NotNullWhen(true)] out SyntaxNode? scope)
        {
            if (local.TrySingleDeclaration(cancellationToken, out var declaration))
            {
                if (declaration is ForEachStatementSyntax forEachStatement)
                {
                    scope = forEachStatement.Statement;
                    return true;
                }

                if (declaration.TryFirstAncestor<AnonymousFunctionExpressionSyntax>(out var lambda))
                {
                    scope = lambda;
                    return true;
                }

                if (declaration.TryFirstAncestor<LocalFunctionStatementSyntax>(out var localFunction))
                {
                    scope = localFunction;
                    return true;
                }

                if (declaration.TryFirstAncestor<GlobalStatementSyntax>(out var global))
                {
                    scope = global.Parent;
                    return true;
                }

                if (declaration.TryFirstAncestor<MemberDeclarationSyntax>(out var member))
                {
                    scope = member;
                    return true;
                }
            }

            scope = null;
            return false;
        }
    }
}

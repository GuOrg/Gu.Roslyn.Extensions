namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper methods for working with <see cref="ISymbol"/>
    /// </summary>
    public static partial class SymbolExt
    {
        /// <summary>
        /// Check if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/>
        /// </summary>
        /// <typeparam name="T1">The first type to check for.</typeparam>
        /// <typeparam name="T2">The second type to check for.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/></param>
        /// <returns>True if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/></returns>
        public static bool IsEither<T1, T2>(this ISymbol symbol)
            where T1 : ISymbol
            where T2 : ISymbol
        {
            return symbol is T1 || symbol is T2;
        }

        /// <summary>
        /// Check if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/> or <typeparamref name="T3"/>
        /// </summary>
        /// <typeparam name="T1">The first type to check for.</typeparam>
        /// <typeparam name="T2">The second type to check for.</typeparam>
        /// <typeparam name="T3">The third type to check for.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/></param>
        /// <returns>True if <paramref name="symbol"/> is either <typeparamref name="T1"/> or <typeparamref name="T2"/> or <typeparamref name="T3"/></returns>
        public static bool IsEither<T1, T2, T3>(this ISymbol symbol)
            where T1 : ISymbol
            where T2 : ISymbol
            where T3 : ISymbol
        {
            return symbol is T1 || symbol is T2 || symbol is T3;
        }

        /// <summary>
        /// Try to get the scope where <paramref name="local"/> is visible
        /// </summary>
        /// <param name="local">The <see cref="ILocalSymbol"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <param name="scope">The scope</param>
        /// <returns>True if a scope could be determined.</returns>
        public static bool TryGetScope(this ILocalSymbol local, CancellationToken cancellationToken, out SyntaxNode scope)
        {
            if (local.TrySingleDeclaration(cancellationToken, out var declaration))
            {
                if (declaration.FirstAncestor<AnonymousFunctionExpressionSyntax>() is SyntaxNode lambda)
                {
                    scope = lambda;
                    return true;
                }

                if (declaration.FirstAncestor<MemberDeclarationSyntax>() is SyntaxNode member)
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

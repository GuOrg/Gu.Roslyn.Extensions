namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxToken"/>.
    /// </summary>
    public static class SyntaxTokenExt
    {
        /// <summary>
        /// Check if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxToken"/>.</param>
        /// <param name="kind1">The first kind.</param>
        /// <param name="kind2">The other kind.</param>
        /// <returns>True if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>. </returns>
        public static bool IsEither(this SyntaxToken node, SyntaxKind kind1, SyntaxKind kind2) => node.IsKind(kind1) || node.IsKind(kind2);

        /// <summary>
        /// Check if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/> or <paramref name="kind3"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxToken"/>.</param>
        /// <param name="kind1">The first kind.</param>
        /// <param name="kind2">The other kind.</param>
        /// <param name="kind3">The third kind.</param>
        /// <returns>True if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>. </returns>
        public static bool IsEither(this SyntaxToken node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3) => node.IsKind(kind1) || node.IsKind(kind2) || node.IsKind(kind3);

        /// <summary>
        /// Get the <see cref="FileLinePositionSpan"/> for the token in the containing document.
        /// </summary>
        /// <param name="token">The <see cref="SyntaxToken"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="FileLinePositionSpan"/> for the token in the containing document.</returns>
        public static FileLinePositionSpan FileLinePositionSpan(this SyntaxToken token, CancellationToken cancellationToken)
        {
            if (token.SyntaxTree is null)
            {
                throw new ArgumentException("token is not in a syntax tree.", nameof(token));
            }

            return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken);
        }
    }
}

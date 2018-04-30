namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxNode"/>
    /// </summary>
    public static class SyntaxNodeExt
    {
        /// <summary>
        /// Get the <see cref="FileLinePositionSpan"/> for the token in the containing document.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>The <see cref="FileLinePositionSpan"/> for the token in the containing document.</returns>
        public static FileLinePositionSpan FileLinePositionSpan(this SyntaxNode node, CancellationToken cancellationToken)
        {
            return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken);
        }

        /// <summary>
        /// Check if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="kind1">The first kind</param>
        /// <param name="kind2">The other kind</param>
        /// <returns>True if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/> </returns>
        public static bool IsEither(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2) => node.IsKind(kind1) || node.IsKind(kind2);

        /// <summary>
        /// Try getting <see cref="SyntaxNodeExt.FirstAncestor{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of ancestor</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="result"><see cref="SyntaxNodeExt.FirstAncestor{T}"/></param>
        /// <returns>True if not null</returns>
        public static bool TryFirstAncestor<T>(this SyntaxNode node, out T result)
            where T : SyntaxNode
        {
            result = node.FirstAncestor<T>();
            return result != null;
        }

        /// <summary>
        /// Try getting <see cref="SyntaxNode.FirstAncestorOrSelf{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of ancestor</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <param name="result"><see cref="SyntaxNode.FirstAncestorOrSelf{T}"/></param>
        /// <returns>True if not null</returns>
        public static bool TryFirstAncestorOrSelf<T>(this SyntaxNode node, out T result)
            where T : SyntaxNode
        {
            result = node.FirstAncestorOrSelf<T>();
            return result != null;
        }

        /// <summary>
        /// Same as <see cref="SyntaxNode.FirstAncestorOrSelf{T}"/> but for strict ancestors.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/></param>
        /// <returns>The ancestor or null</returns>
        public static T FirstAncestor<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            if (node == null)
            {
                return null;
            }

            if (node is T)
            {
                return node.Parent?.FirstAncestorOrSelf<T>();
            }

            return node.FirstAncestorOrSelf<T>();
        }

        /// <summary>
        /// Check if the node is in an expression tree.
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
        /// <returns>True if the node is in an expression tree.</returns>
        public static bool IsInExpressionTree(this SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                return false;
            }

            while (node.TryFirstAncestor<AnonymousFunctionExpressionSyntax>(out var lambda))
            {
                if (semanticModel.GetTypeInfoSafe(lambda, cancellationToken) is TypeInfo typeInfo &&
                    typeInfo.ConvertedType.IsAssignableTo(QualifiedType.System.Linq.Expression, semanticModel.Compilation))
                {
                    return true;
                }

                node = lambda;
            }

            return false;
        }

        /// <summary>
        /// Tries to determine if <paramref name="node"/> is executed before <paramref name="other"/>
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <returns>Null if it could not be determined.</returns>
        public static bool? IsExecutedBefore(this SyntaxNode node, SyntaxNode other)
        {
            if (node is null ||
                other is null)
            {
                return false;
            }

            if (!node.SharesAncestor<MemberDeclarationSyntax>(other))
            {
                return null;
            }

            if (other.FirstAncestor<AnonymousFunctionExpressionSyntax>() is AnonymousFunctionExpressionSyntax otherLambda)
            {
                if (node.FirstAncestor<AnonymousFunctionExpressionSyntax>() is AnonymousFunctionExpressionSyntax nodeLambda)
                {
                    if (ReferenceEquals(nodeLambda, otherLambda))
                    {
                        return IsBeforeInScopeCore();
                    }

                    return null;
                }

                return node.SpanStart < otherLambda.SpanStart ? true : (bool?)null;
            }
            else if (node.FirstAncestor<AnonymousFunctionExpressionSyntax>() is AnonymousFunctionExpressionSyntax nodeLambda)
            {
                if (other.SpanStart < nodeLambda.SpanStart)
                {
                    return false;
                }

                return null;
            }

            return IsBeforeInScopeCore();

            bool? IsBeforeInScopeCore()
            {
                if (node.Contains(other) &&
                    node.SpanStart < other.SpanStart)
                {
                    return true;
                }

                var statement = node.FirstAncestorOrSelf<StatementSyntax>();
                var otherStatement = other.FirstAncestorOrSelf<StatementSyntax>();
                if (statement == null ||
                    otherStatement == null)
                {
                    return null;
                }

                var block = statement.Parent as BlockSyntax;
                var otherBlock = otherStatement.Parent as BlockSyntax;
                if (block == null && otherBlock == null)
                {
                    return false;
                }

                if (ReferenceEquals(block, otherBlock) ||
                    otherBlock?.Contains(node) == true ||
                    block?.Contains(other) == true)
                {
                    var firstAnon = FirstAncestor<AnonymousFunctionExpressionSyntax>(node);
                    var otherAnon = FirstAncestor<AnonymousFunctionExpressionSyntax>(other);
                    if (!ReferenceEquals(firstAnon, otherAnon))
                    {
                        return true;
                    }

                    return statement.SpanStart < otherStatement.SpanStart;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the nodes shares an ancestor of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="first">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <returns>True if a common ancestor was found.</returns>
        public static bool SharesAncestor<T>(this SyntaxNode first, SyntaxNode other)
            where T : SyntaxNode
        {
            var firstAncestor = FirstAncestor<T>(first);
            var otherAncestor = FirstAncestor<T>(other);
            if (firstAncestor == null ||
                otherAncestor == null)
            {
                return false;
            }

            return firstAncestor == otherAncestor;
        }
    }
}

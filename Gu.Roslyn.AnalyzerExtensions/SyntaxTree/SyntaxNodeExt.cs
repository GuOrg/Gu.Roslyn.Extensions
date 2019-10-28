namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxNode"/>.
    /// </summary>
    public static class SyntaxNodeExt
    {
        /// <summary>
        /// Get the <see cref="FileLinePositionSpan"/> for the token in the containing document.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="FileLinePositionSpan"/> for the token in the containing document.</returns>
        public static FileLinePositionSpan FileLinePositionSpan(this SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken);
        }

        /// <summary>
        /// Check if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="kind1">The first kind.</param>
        /// <param name="kind2">The other kind.</param>
        /// <returns>True if <paramref name="node"/> is either of <paramref name="kind1"/> or <paramref name="kind2"/>. </returns>
        public static bool IsEither(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2) => node.IsKind(kind1) || node.IsKind(kind2);

        /// <summary>
        /// Try getting <see cref="SyntaxNodeExt.FirstAncestor{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="result"><see cref="SyntaxNodeExt.FirstAncestor{T}"/>.</param>
        /// <returns>True if not null.</returns>
        public static bool TryFirstAncestor<T>(this SyntaxNode node, [NotNullWhen(true)]out T? result)
            where T : SyntaxNode
        {
            result = node.FirstAncestor<T>();
            return result != null;
        }

        /// <summary>
        /// Try getting <see cref="SyntaxNode.FirstAncestorOrSelf{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="result"><see cref="SyntaxNode.FirstAncestorOrSelf{T}"/>.</param>
        /// <returns>True if not null.</returns>
        public static bool TryFirstAncestorOrSelf<T>(this SyntaxNode node, [NotNullWhen(true)]out T? result)
            where T : SyntaxNode
        {
            result = node?.FirstAncestorOrSelf<T>();
            return result != null;
        }

        /// <summary>
        /// Same as <see cref="SyntaxNode.FirstAncestorOrSelf{T}"/> but for strict ancestors.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <returns>The ancestor or null.</returns>
        public static T? FirstAncestor<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            if (node is null)
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
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if the node is in an expression tree.</returns>
        public static bool IsInExpressionTree(this SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (semanticModel is null)
            {
                throw new System.ArgumentNullException(nameof(semanticModel));
            }

            if (node is null)
            {
                return false;
            }

            while (node.TryFirstAncestor<AnonymousFunctionExpressionSyntax>(out var lambda))
            {
                if (semanticModel.GetTypeInfoSafe(lambda, cancellationToken) is TypeInfo typeInfo &&
                    typeInfo.ConvertedType is ITypeSymbol convertedType &&
                    convertedType.IsAssignableTo(QualifiedType.System.Linq.Expression, semanticModel.Compilation))
                {
                    return true;
                }

                node = lambda;
            }

            return false;
        }

        /// <summary>
        /// Check if the first ancestor of type <typeparamref name="T"/> is the same instance.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="first">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>True if a common ancestor was found.</returns>
        public static bool SharesAncestor<T>(this SyntaxNode first, SyntaxNode other, [NotNullWhen(true)]out T? ancestor)
            where T : SyntaxNode
        {
            var firstAncestor = FirstAncestor<T>(first);
            var otherAncestor = FirstAncestor<T>(other);
            if (firstAncestor is null ||
                otherAncestor is null ||
                !ReferenceEquals(firstAncestor, otherAncestor))
            {
                ancestor = null;
                return false;
            }

            ancestor = firstAncestor;
            return true;
        }

        /// <summary>
        /// Check if the first ancestor of type <typeparamref name="T"/> is the same instance.
        /// </summary>
        /// <typeparam name="T">The type of ancestor.</typeparam>
        /// <param name="first">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>True if a common ancestor was found.</returns>
        public static bool TryFindSharedAncestorRecursive<T>(this SyntaxNode first, SyntaxNode other, [NotNullWhen(true)]out T? ancestor)
            where T : SyntaxNode
        {
            var firstAncestor = FirstAncestor<T>(first);
            var otherAncestor = FirstAncestor<T>(other);
            if (firstAncestor is null ||
                otherAncestor is null)
            {
                ancestor = null;
                return false;
            }

            if (ReferenceEquals(firstAncestor, otherAncestor) ||
                firstAncestor.Contains(otherAncestor))
            {
                ancestor = firstAncestor;
                return true;
            }

            if (otherAncestor.Contains(firstAncestor))
            {
                ancestor = otherAncestor;
                return true;
            }

            return TryFindSharedAncestorRecursive(firstAncestor, otherAncestor, out ancestor);
        }
    }
}

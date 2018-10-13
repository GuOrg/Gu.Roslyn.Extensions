namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ExpressionSyntax"/>.
    /// </summary>
    public static class ExpressionSyntaxExt
    {
        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsAssignableTo(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            return semanticModel.ClassifyConversion(expression, destination).IsImplicit;
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="QualifiedType"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsAssignableTo(this ExpressionSyntax expression, QualifiedType destination, SemanticModel semanticModel)
        {
            if (expression == null ||
                destination == null ||
                semanticModel?.Compilation == null)
            {
                return false;
            }

            return IsAssignableTo(expression, destination.GetTypeSymbol(semanticModel.Compilation), semanticModel);
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsSameType(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            return semanticModel.ClassifyConversion(expression, destination).IsIdentity;
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="destination">The other <see cref="QualifiedType"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/>. </returns>
        public static bool IsSameType(this ExpressionSyntax expression, QualifiedType destination, SemanticModel semanticModel)
        {
            if (expression == null ||
                destination == null ||
                semanticModel?.Compilation == null)
            {
                return false;
            }

            return IsSameType(expression, destination.GetTypeSymbol(semanticModel.Compilation), semanticModel);
        }

        /// <summary>
        /// Check if (destination)(object)expression will work.
        /// </summary>
        /// <param name="expression">The expression containing the value.</param>
        /// <param name="destination">The type to cast to.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True if a boxed instance can be cast.</returns>
        public static bool IsRepresentationPreservingConversion(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return semanticModel.IsRepresentationPreservingConversion(expression, destination, cancellationToken);
        }

        /// <summary>
        /// Tries to determine if <paramref name="node"/> is executed before <paramref name="other"/>.
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="other">The second node.</param>
        /// <returns>Null if it could not be determined.</returns>
        public static bool? IsExecutedBefore(this ExpressionSyntax node, ExpressionSyntax other)
        {
            if (node is null ||
                other is null)
            {
                return false;
            }

            if (node.TryFirstAncestor(out AnonymousFunctionExpressionSyntax nodeLambda))
            {
                if (other.TryFirstAncestor(out AnonymousFunctionExpressionSyntax otherLambda))
                {
                    if (!ReferenceEquals(nodeLambda, otherLambda))
                    {
                        return null;
                    }

                    if (!(nodeLambda.Body is BlockSyntax))
                    {
                        return node.SpanStart < other.SpanStart;
                    }
                }
                else
                {
                    if (node.SpanStart > other.SpanStart)
                    {
                        return false;
                    }

                    return null;
                }
            }

            else if (other.TryFirstAncestor<AnonymousFunctionExpressionSyntax>(out _))
            {
                if (node.SpanStart < other.SpanStart)
                {
                    return true;
                }

                return null;
            }

            if (node.TryFirstAncestor(out StatementSyntax statement) &&
                other.TryFirstAncestor(out StatementSyntax otherStatement))
            {
                if (ReferenceEquals(statement, otherStatement))
                {
                    return node.SpanStart < other.SpanStart;
                }

                return statement.IsExecutedBefore(otherStatement);
            }

            if (node.TryFindSharedAncestorRecursive(other, out BinaryExpressionSyntax _))
            {
                return node.SpanStart < other.SpanStart;
            }

            return null;
        }
    }
}

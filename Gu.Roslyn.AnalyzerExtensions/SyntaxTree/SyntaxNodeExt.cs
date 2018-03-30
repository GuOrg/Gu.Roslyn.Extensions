namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class SyntaxNodeExt
    {
        public static bool IsEitherKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2) => node.IsKind(kind1) || node.IsKind(kind2);

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

        public static bool IsInExpressionTree(this SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var lambda = node.FirstAncestor<LambdaExpressionSyntax>();
            while (lambda != null)
            {
                var lambdaType = semanticModel.GetTypeInfoSafe(lambda, cancellationToken).ConvertedType;
                if (lambdaType != null &&
                    lambdaType.Is(KnownSymbol.Expression))
                {
                    return true;
                }

                lambda = lambda.FirstAncestor<LambdaExpressionSyntax>();
            }

            return false;
        }

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

        internal static bool SharesAncestor<T>(this SyntaxNode first, SyntaxNode other)
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

namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class Trivia
    {
        public static SyntaxNode WithTriviaFrom(this SyntaxNode node, SyntaxNode other)
        {
            return node.WithLeadingTriviaFrom(other)
                .WithTrailingTriviaFrom(other);
        }

        public static SyntaxNode WithLeadingTriviaFrom(this SyntaxNode node, SyntaxNode other)
        {
            return other.HasLeadingTrivia
                ? node.WithLeadingTrivia(other.GetLeadingTrivia())
                : node;
        }

        public static SyntaxNode WithTrailingTriviaFrom(this SyntaxNode node, SyntaxNode other)
        {
            return other.HasTrailingTrivia
                ? node.WithTrailingTrivia(other.GetTrailingTrivia())
                : node;
        }

        public static T WithLeadingElasticLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.ElasticLineFeed));
            }

            return node.WithLeadingTrivia(SyntaxFactory.ElasticLineFeed);
        }

        public static T WithLeadingLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasLeadingTrivia)
            {
                return node.WithLeadingTrivia(
                    node.GetLeadingTrivia()
                        .Insert(0, SyntaxFactory.LineFeed));
            }

            return node.WithLeadingTrivia(SyntaxFactory.LineFeed);
        }

        public static T WithTrailingElasticLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(
                    node.GetTrailingTrivia()
                        .Add(SyntaxFactory.ElasticLineFeed));
            }

            return node.WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
        }

        public static T WithTrailingLineFeed<T>(this T node)
            where T : SyntaxNode
        {
            if (node.HasTrailingTrivia)
            {
                return node.WithTrailingTrivia(
                    node.GetTrailingTrivia()
                        .Add(SyntaxFactory.LineFeed));
            }

            return node.WithTrailingTrivia(SyntaxFactory.LineFeed);
        }
    }
}

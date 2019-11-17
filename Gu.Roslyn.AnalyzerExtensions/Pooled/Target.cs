namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public static class Target
    {
        public static Target<TSymbol, TNode> Create<TSymbol, TNode>(TSymbol symbol, TNode? node)
        where TSymbol : ISymbol
        where TNode : SyntaxNode
        {
            return new Target<TSymbol, TNode>(symbol, node);
        }
    }
}

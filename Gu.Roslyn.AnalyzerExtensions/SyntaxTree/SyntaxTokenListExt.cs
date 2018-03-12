namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class SyntaxTokenListExt
    {
        public static bool Any(this SyntaxTokenList list, SyntaxKind k1, SyntaxKind k2) => list.Any(k1) || list.Any(k2);
    }
}
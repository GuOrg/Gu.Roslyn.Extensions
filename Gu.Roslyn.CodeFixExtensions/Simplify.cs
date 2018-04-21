namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Simplification;

    public static class Simplify
    {
        public static T WithSimplifiedNames<T>(this T node)
            where T : SyntaxNode
        {
            return (T)SimplifyNamesRewriter.Default.Visit(node);
        }

        private class SimplifyNamesRewriter : CSharpSyntaxRewriter
        {
            public static readonly SimplifyNamesRewriter Default = new SimplifyNamesRewriter();

            public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
            {
                return base.VisitQualifiedName(node).WithAdditionalAnnotations(Simplifier.Annotation);
            }
        }
    }
}

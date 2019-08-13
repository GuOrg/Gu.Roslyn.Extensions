namespace Gu.Roslyn.CodeFixExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Simplification;

    /// <summary>
    /// Adds <see cref="Simplifier.Annotation"/> to all <see cref="QualifiedNameSyntax"/>.
    /// </summary>
    public class SimplifyNamesRewriter : CSharpSyntaxRewriter
    {
        /// <summary>The default instance.</summary>
        public static readonly SimplifyNamesRewriter Default = new SimplifyNamesRewriter();

        /// <inheritdoc />
        public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
        {
            return base.VisitQualifiedName(node).WithAdditionalAnnotations(Simplifier.Annotation);
        }
    }
}

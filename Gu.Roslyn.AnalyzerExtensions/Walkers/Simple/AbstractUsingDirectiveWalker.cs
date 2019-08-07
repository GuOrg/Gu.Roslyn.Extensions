namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class AbstractUsingDirectiveWalker<T> : PooledWalker<T>
        where T : PooledWalker<T>
    {
        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Stop walking here
        }

        /// <inheritdoc />
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            // Stop walking here
        }

        /// <inheritdoc/>
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // Stop walking here
        }
    }
}
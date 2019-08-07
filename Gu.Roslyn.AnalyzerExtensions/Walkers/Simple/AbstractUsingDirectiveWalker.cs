namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Base class for walkers checking <see cref="UsingDirectiveSyntax"/>.
    /// Stops walking at <see cref="BaseTypeDeclarationSyntax"/>.
    /// </summary>
    /// <typeparam name="T">The walker type.</typeparam>
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

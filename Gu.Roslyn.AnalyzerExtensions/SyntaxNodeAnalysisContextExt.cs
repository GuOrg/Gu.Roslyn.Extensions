namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxNodeAnalysisContext"/>.
    /// </summary>
    public static class SyntaxNodeAnalysisContextExt
    {
        /// <summary>
        /// Check if the current node should be analyzed.
        /// </summary>
        /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/>.</param>
        /// <returns>True if the current node should be skipped.</returns>
        public static bool IsExcludedFromAnalysis(this SyntaxNodeAnalysisContext context)
        {
            if (context.Node == null ||
                context.Node.IsMissing ||
                context.SemanticModel == null)
            {
                return true;
            }

            return IsGenerated(context.ContainingSymbol) ||
                   context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.i.cs") ||
                   context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.cs");

            bool IsGenerated(ISymbol symbol)
            {
                if (symbol == null)
                {
                    return false;
                }

                foreach (var attribute in symbol.GetAttributes())
                {
                    if (attribute.AttributeClass == QualifiedType.System.CodeDom.Compiler.GeneratedCodeAttribute ||
                        attribute.AttributeClass == QualifiedType.System.Runtime.CompilerServices.CompilerGeneratedAttribute)
                    {
                        return true;
                    }

                    return IsGenerated(symbol.ContainingSymbol);
                }

                return false;
            }
        }
    }
}

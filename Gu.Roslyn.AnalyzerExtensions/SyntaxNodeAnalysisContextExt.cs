namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helpers for working with <see cref="SyntaxNodeAnalysisContext"/>
    /// </summary>
    public static class SyntaxNodeAnalysisContextExt
    {
        /// <summary>
        /// Check if the current node should be analyzed.
        /// </summary>
        /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/></param>
        /// <returns>True if the current node should be skipped.</returns>
        public static bool IsExcludedFromAnalysis(this SyntaxNodeAnalysisContext context)
        {
            if (context.Node == null ||
                context.Node.IsMissing ||
                context.SemanticModel == null)
            {
                return true;
            }

            return context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.i.cs") ||
                   context.SemanticModel.SyntaxTree.FilePath.EndsWith(".g.cs");
        }
    }
}

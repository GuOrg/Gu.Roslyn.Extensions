namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for working with <see cref="DiagnosticDescriptor"/>
    /// </summary>
    public static class DiagnosticDescriptorExt
    {
        /// <summary>
        /// Check if the descriptor is currently suppressed.
        /// </summary>
        /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <returns>True if the descriptor is currently suppressed.</returns>
        public static bool IsSuppressed(this DiagnosticDescriptor descriptor, SemanticModel semanticModel)
        {
            if (semanticModel.Compilation.Options.SpecificDiagnosticOptions.TryGetValue(descriptor.Id, out var report))
            {
                return report == ReportDiagnostic.Suppress;
            }

            return !descriptor.IsEnabledByDefault;
        }
    }
}

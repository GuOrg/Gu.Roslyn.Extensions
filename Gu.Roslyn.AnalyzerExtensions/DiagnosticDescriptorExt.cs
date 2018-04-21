namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public static class DiagnosticDescriptorExt
    {
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
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <inheritdoc />
    public abstract class SemanticModelCacheAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "SemanticModelCacheAnalyzer",
            title: "Controls if Semantic models should be cached for syntax trees.",
            messageFormat: "Cache<SyntaxTree, SemanticModel>",
            category: "SemanticModelCacheAnalyzer",
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Controls if Semantic models should be cached for syntax trees.\r\n" +
                         "This can speed up analysis significantly but means Visual Studio uses more memory during compilation.");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.CacheToCompilationEnd<SyntaxTree, SemanticModel>();
        }
    }
}

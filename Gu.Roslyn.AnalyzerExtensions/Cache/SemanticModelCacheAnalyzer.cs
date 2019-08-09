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
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
#pragma warning disable SA1118 // Parameter should not span multiple lines
            description: "Controls if Semantic models should be cached for syntax trees.\r\n" +
                         "This can speed up analysis significantly but means Visual Studio uses more memory during compilation.");
#pragma warning restore SA1118 // Parameter should not span multiple lines

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(x =>
            {
                var transaction = Cache<SemanticModel>.Begin(x.Compilation);
                x.RegisterCompilationEndAction(_ => transaction.Dispose());
            });
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <inheritdoc />
    public abstract class SyntaxTreeCacheAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "SyntaxTreeCacheAnalyzer",
            title: "Controls caching of for example semantic models for syntax trees.",
            messageFormat: "Controls caching of for example Semantic models for syntax trees.",
            category: "Caching",
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
#pragma warning disable SA1118 // Parameter should not span multiple lines
            description: "Controls if Semantic models should be cached for syntax trees.\r\n" +
                         "This can speed up analysis significantly but means Visual Studio uses more memory during compilation.");
#pragma warning restore SA1118 // Parameter should not span multiple lines

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("AnalyzerPerformance", "RS1013:Start action has no registered non-end actions.", Justification = "We want it like this here.")]
        public override void Initialize(AnalysisContext context)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(x =>
#pragma warning restore CA1062 // Validate arguments of public methods
            {
                var transaction = SyntaxTreeCache<SemanticModel>.Begin(x.Compilation);
                x.RegisterCompilationEndAction(_ => transaction.Dispose());
            });
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for working with <see cref="ParameterSyntax"/>.
    /// </summary>
    public static class ParameterSyntaxExt
    {
        /// <summary>
        /// Check if <paramref name="node"/> is <paramref name="type"/>.
        /// Optimized so that the stuff that can be checked in syntax mode is done before calling get symbol.
        /// </summary>
        /// <param name="node">The <see cref="ParameterSyntax"/>.</param>
        /// <param name="type">The <see cref="QualifiedType"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>True  if <paramref name="node"/> is <paramref name="type"/>.</returns>
        public static bool IsType(this ParameterSyntax node, QualifiedType type, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return node switch
            {
                { Type: IdentifierNameSyntax identifierName } => identifierName == type &&
                                                                 semanticModel.TryGetType(node, cancellationToken, out var symbol) &&
                                                                 symbol == type,
                _ => semanticModel.TryGetType(node, cancellationToken, out var symbol) &&
                     symbol == type,
            };
        }
    }
}

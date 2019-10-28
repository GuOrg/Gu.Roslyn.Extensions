namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Extension methods for <see cref="ArgumentSyntax"/>.
    /// </summary>
    public static class AttributeArgumentSyntaxExt
    {
        /// <summary>
        /// Try get the value of the argument if it is a constant string.
        /// </summary>
        /// <param name="argument">The <see cref="AttributeArgumentSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The string contents of <paramref name="argument"/>.</param>
        /// <returns>True if the argument expression was a constant string.</returns>
        public static bool TryGetStringValue(this AttributeArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out string? result)
        {
            result = null;
            return argument?.Expression is ExpressionSyntax expression &&
                   expression.TryGetStringValue(semanticModel, cancellationToken, out result);
        }

        /// <summary>
        /// Try get the value of the argument if it is a typeof() call.
        /// </summary>
        /// <param name="argument">The <see cref="AttributeArgumentSyntax"/>.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The string contents of <paramref name="argument"/>.</param>
        /// <returns>True if the call is typeof() and we could figure out the type.</returns>
        public static bool TryGetTypeofValue(this AttributeArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)]out ITypeSymbol? result)
        {
            result = null;
            return argument?.Expression is TypeOfExpressionSyntax expression &&
                   semanticModel.TryGetType(expression.Type, cancellationToken, out result);
        }
    }
}

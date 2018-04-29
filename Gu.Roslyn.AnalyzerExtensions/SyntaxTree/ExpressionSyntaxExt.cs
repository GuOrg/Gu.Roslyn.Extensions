namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class ExpressionSyntaxExt
    {
        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/> </returns>
        public static bool Is(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            var conversion = semanticModel.ClassifyConversion(expression, destination);
            return conversion.IsIdentity ||
                   conversion.IsImplicit;
        }

        /// <summary>
        /// Check if <paramref name="expression"/> is <paramref name="destination"/>
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
        /// <param name="destination">The other <see cref="ITypeSymbol"/></param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/></param>
        /// <returns>True if <paramref name="expression"/> is <paramref name="destination"/> </returns>
        public static bool IsSameType(this ExpressionSyntax expression, ITypeSymbol destination, SemanticModel semanticModel)
        {
            if (expression == null || destination == null)
            {
                return false;
            }

            var conversion = semanticModel.ClassifyConversion(expression, destination);
            return conversion.IsIdentity;
        }
    }
}

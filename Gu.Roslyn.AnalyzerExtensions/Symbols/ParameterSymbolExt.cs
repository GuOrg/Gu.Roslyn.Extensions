namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Helpers for working with <see cref="IParameterSymbol"/>
    /// </summary>
    public static class ParameterSymbolExt
    {
        /// <summary>
        /// Check if the parameter has [CallerMemberName]
        /// </summary>
        /// <param name="parameter">The <see cref="IParameterSymbol"/></param>
        /// <returns>True if the parameter has [CallerMemberName]</returns>
        public static bool IsCallerMemberName(this IParameterSymbol parameter)
        {
            if (parameter.HasExplicitDefaultValue &&
                parameter.Type == QualifiedType.System.String)
            {
                foreach (var attribute in parameter.GetAttributes())
                {
                    if (attribute.AttributeClass == QualifiedType.System.Runtime.CompilerServices.CallerMemberNameAttribute)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

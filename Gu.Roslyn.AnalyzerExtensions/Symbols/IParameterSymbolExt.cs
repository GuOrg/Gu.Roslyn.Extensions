namespace Gu.Roslyn.AnalyzerExtensions;

using Microsoft.CodeAnalysis;

/// <summary>
/// Helpers for working with <see cref="IParameterSymbol"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IParameterSymbolExt
{
    /// <summary>
    /// Check if the parameter has [CallerMemberName].
    /// </summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/>.</param>
    /// <returns>True if the parameter has [CallerMemberName].</returns>
    public static bool IsCallerMemberName(this IParameterSymbol parameter)
    {
        if (parameter is null)
        {
            throw new System.ArgumentNullException(nameof(parameter));
        }

        if (parameter is { Type.SpecialType: SpecialType.System_String, HasExplicitDefaultValue: true })
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

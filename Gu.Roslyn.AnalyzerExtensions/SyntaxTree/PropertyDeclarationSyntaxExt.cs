namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class PropertyDeclarationSyntaxExt
    {
        public static bool IsGetOnly(this BasePropertyDeclarationSyntax property)
        {
            if (property.TryGetGetter(out var getter) &&
                getter.Body == null &&
                getter.ExpressionBody == null)
            {
                return !property.TryGetSetter(out _);
            }

            return false;
        }

        public static bool IsAutoProperty(this BasePropertyDeclarationSyntax property)
        {
            if (property.TryGetGetter(out var getter) &&
                getter.Body == null &&
                getter.ExpressionBody == null)
            {
                if (property.TryGetSetter(out var setter))
                {
                    return setter.Body == null &&
                           setter.ExpressionBody == null;
                }

                return true;
            }

            return false;
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static partial class TypeSyntaxExt
    {
        public static bool IsVoid(this TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefinedType)
            {
                return predefinedType.Keyword.ValueText == "void";
            }

            return false;
        }
    }
}

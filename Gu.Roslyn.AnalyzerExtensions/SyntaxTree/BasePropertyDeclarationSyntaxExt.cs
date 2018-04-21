namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class BasePropertyDeclarationSyntaxExt
    {
        public static bool TryGetGetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.GetAccessorDeclaration), out result) == true;
        }

        public static bool TryGetSetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.SetAccessorDeclaration), out result) == true;
        }
    }
}

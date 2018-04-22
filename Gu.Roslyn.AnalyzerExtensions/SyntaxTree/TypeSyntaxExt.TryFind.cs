// ReSharper disable UnusedMember.Global
namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static partial class TypeSyntaxExt
    {
        public static bool TryFindField(this TypeDeclarationSyntax type, string name, out FieldDeclarationSyntax match)
        {
            match = null;
            if (type == null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is FieldDeclarationSyntax declaration &&
                    declaration.Declaration.Variables.TrySingle(x => x.Identifier.ValueText == name, out _))
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        public static bool TryFindMethod(this TypeDeclarationSyntax type, string name, out MethodDeclarationSyntax match)
        {
            match = null;
            if (type == null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is MethodDeclarationSyntax declaration &&
                    declaration.Identifier.ValueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }

        public static bool TryFindProperty(this TypeDeclarationSyntax type, string name, out PropertyDeclarationSyntax match)
        {
            match = null;
            if (type == null)
            {
                return false;
            }

            foreach (var member in type.Members)
            {
                if (member is PropertyDeclarationSyntax declaration &&
                    declaration.Identifier.ValueText == name)
                {
                    match = declaration;
                    return true;
                }
            }

            return false;
        }
    }
}
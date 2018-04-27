namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class Parse
    {
        public static FieldDeclarationSyntax FieldDeclaration(string code)
        {
            return (FieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public static ConstructorDeclarationSyntax ConstructorDeclaration(string code)
        {
            return (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public static EventFieldDeclarationSyntax EventFieldDeclaration(string code)
        {
            return (EventFieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public static EventDeclarationSyntax EventDeclaration(string code)
        {
            return (EventDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public static PropertyDeclarationSyntax PropertyDeclaration(string code)
        {
            return (PropertyDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public static MethodDeclarationSyntax MethodDeclaration(string code)
        {
            return (MethodDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class Parse
    {
        public FieldDeclarationSyntax FieldDeclaration(string code)
        {
            return (FieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public ConstructorDeclarationSyntax ConstructorDeclaration(string code)
        {
            return (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public EventFieldDeclarationSyntax EventFieldDeclaration(string code)
        {
            return (EventFieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public EventDeclarationSyntax EventDeclaration(string code)
        {
            return (EventDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public PropertyDeclarationSyntax PropertyDeclaration(string code)
        {
            return (PropertyDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        public MethodDeclarationSyntax MethodDeclaration(string code)
        {
            return (MethodDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }
    }
}

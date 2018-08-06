namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class ParseTests
    {
        [TestCase("private readonly int value = 1;")]
        public void FieldDeclaration(string code)
        {
            var declaration = Parse.FieldDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
            var expected = (FieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
            AnalyzerAssert.Ast(expected, declaration);
        }

        [TestCase("public Foo(){}")]
        public void ConstructorDeclaration(string code)
        {
            var declaration = Parse.ConstructorDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
            var expected = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
            AnalyzerAssert.Ast(expected, declaration);
        }

        [TestCase("public int Foo { get; }")]
        public void PropertyDeclaration(string code)
        {
            var declaration = Parse.PropertyDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
            var expected = (PropertyDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
            AnalyzerAssert.Ast(expected, declaration);
        }

        [TestCase("public int Foo() => 1;")]
        public void MethodDeclaration(string code)
        {
            var declaration = Parse.MethodDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
            var expected = (MethodDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
            AnalyzerAssert.Ast(expected, declaration);
        }

        [TestCase("<summary> Text </summary>")]
        [TestCase("<param name=\"cancellationToken\">The <see cref=\"CancellationToken\"/> that the task will observe.</param>")]
        public void XmlElementSyntaxSingleLine(string code)
        {
            var node = Parse.XmlElementSyntax(code, string.Empty);
            Assert.AreEqual(code, node.ToFullString());
            var expected = SyntaxFactory.ParseLeadingTrivia("/// " + code)
                                        .Single(x => x.HasStructure)
                                        .GetStructure()
                                        .ChildNodes()
                                        .OfType<XmlElementSyntax>()
                                        .Single();
            AnalyzerAssert.Ast(expected, node);
        }

        [TestCase("<summary> Line 1\r\nLine2 </summary>", "<summary> Line 1\r\n/// Line2 </summary>")]
        [TestCase("<summary>\r\nLine 1\r\n</summary>", "<summary>\r\n/// Line 1\r\n/// </summary>")]
        [TestCase("<summary>\r\nLine 1\r\nLine2\r\n</summary>", "<summary>\r\n/// Line 1\r\n/// Line2\r\n/// </summary>")]
        public void XmlElementSyntaxMultiLine(string code, string expected)
        {
            var node = Parse.XmlElementSyntax(code, string.Empty);
            Assert.AreEqual(expected, node.ToFullString());
        }

        [TestCase("/// <summary> Text </summary>")]
        [TestCase("/// <summary> Line 1\r\n/// Line2 </summary>")]
        [TestCase("/// <summary>\r\n/// Line 1\r\n/// </summary>")]
        [TestCase("/// <summary>\r\n/// Line 1\r\n/// Line2\r\n/// </summary>")]
        [TestCase("/// <param name=\"cancellationToken\">The <see cref=\"CancellationToken\"/> that the task will observe.</param>")]
        [TestCase("/// <summary>\r\n        /// Initializes a new instance of the <see cref=\"Foo\"/> class.\r\n        /// </summary>")]
        [TestCase("/// <summary> Initializes a new instance of the <see cref=\"Foo\"/> class. </summary>")]
        public void DocumentationCommentTriviaSyntax(string code)
        {
            var node = Parse.DocumentationCommentTriviaSyntax(code);
            Assert.AreEqual(code + "\r\n", node.ToFullString());
        }

        [TestCase("/// <summary> Text </summary>")]
        public void LeadingTrivia(string code)
        {
            var node = Parse.LeadingTrivia(code);
            Assert.AreEqual(code + "\r\n", node.ToFullString());
        }
    }
}

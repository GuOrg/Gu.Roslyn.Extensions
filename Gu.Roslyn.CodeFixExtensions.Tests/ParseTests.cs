namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using NUnit.Framework;

    public class ParseTests
    {
        [TestCase("private readonly int value = 1;")]
        public void FieldDeclaration(string code)
        {
            var declaration = Parse.FieldDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }

        [TestCase("public Foo(){}")]
        public void ConstructorDeclaration(string code)
        {
            var declaration = Parse.ConstructorDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }

        [TestCase("public int Foo { get; }")]
        public void PropertyDeclaration(string code)
        {
            var declaration = Parse.PropertyDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }

        [TestCase("public int Foo() => 1;")]
        public void MethodDeclaration(string code)
        {
            var declaration = Parse.MethodDeclaration(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }

        [TestCase("<summary> Text </summary>")]
        [TestCase("<param name=\"cancellationToken\">The <see cref=\"CancellationToken\"/> that the task will observe.</param>")]
        public void XmlElementSyntax(string code)
        {
            var declaration = Parse.XmlElementSyntax(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }

        [TestCase("/// <summary>\r\n        /// Initializes a new instance of the <see cref=\"Foo\"/> class.\r\n        /// </summary>")]
        [TestCase("/// <summary> Initializes a new instance of the <see cref=\"Foo\"/> class. </summary>")]
        public void DocumentationCommentTriviaSyntax(string code)
        {
            var declaration = Parse.DocumentationCommentTriviaSyntax(code);
            Assert.AreEqual(code, declaration.ToFullString());
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using NUnit.Framework;

    public class ParseTests
    {
        [TestCase("private readonly int value = 1;")]
        public void FieldDeclaration(string code)
        {
            var declaration = Parse.FieldDeclaration(code);
            Assert.AreEqual(code, declaration.ToString());
        }

        [TestCase("public Foo(){}")]
        public void ConstructorDeclaration(string code)
        {
            var declaration = Parse.ConstructorDeclaration(code);
            Assert.AreEqual(code, declaration.ToString());
        }

        [TestCase("public int Foo { get; }")]
        public void PropertyDeclaration(string code)
        {
            var declaration = Parse.PropertyDeclaration(code);
            Assert.AreEqual(code, declaration.ToString());
        }

        [TestCase("public int Foo() => 1;")]
        public void MethodDeclaration(string code)
        {
            var declaration = Parse.MethodDeclaration(code);
            Assert.AreEqual(code, declaration.ToString());
        }

        [TestCase("<summary> Text </summary>")]
        public void XmlElementSyntax(string code)
        {
            var declaration = Parse.XmlElementSyntax(code);
            Assert.AreEqual(code, declaration.ToString());
        }
    }
}

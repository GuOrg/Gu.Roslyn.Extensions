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
    }
}

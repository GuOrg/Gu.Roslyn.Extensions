namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class QualifiedTypeTests
    {
        [Test]
        public void SymbolEquality()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal object Bar()
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("Bar");
            var typeSymbol = semanticModel.GetDeclaredSymbol(node).ReturnType;
            Assert.AreEqual(true, typeSymbol == new QualifiedType("System.Object"));
            Assert.AreEqual(true, typeSymbol == QualifiedType.System.Object);
            Assert.AreEqual(false, typeSymbol == QualifiedType.System.String);
            Assert.AreEqual(false, typeSymbol != new QualifiedType("System.Object"));
            Assert.AreEqual(true, typeSymbol.Is(QualifiedType.System.Object));
            Assert.AreEqual(false, typeSymbol.Is(QualifiedType.System.String));
        }

        [TestCase("Object")]
        [TestCase("System.Object")]
        [TestCase("object")]
        public void TypeSyntaxEquality(string type)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal object Bar()
        {
        }
    }
}".AssertReplace("object", type));
            var typeSyntax = syntaxTree.FindMethodDeclaration("Bar").ReturnType;
            Assert.AreEqual(true, typeSyntax == new QualifiedType("System.Object", "object"));
            Assert.AreEqual(true, typeSyntax == QualifiedType.System.Object);
            Assert.AreEqual(false, typeSyntax == QualifiedType.System.String);
            Assert.AreEqual(false, typeSyntax != new QualifiedType("System.Object", "object"));
        }
    }
}

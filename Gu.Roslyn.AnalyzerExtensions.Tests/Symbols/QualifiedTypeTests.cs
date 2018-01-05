namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class QualifiedTypeTests
    {
        [Test]
        public void GetDeclaredSymbolSafeMethod()
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
            Assert.AreEqual(true, typeSymbol == QualifiedType.Object);
            Assert.AreEqual(false, typeSymbol != new QualifiedType("System.Object"));
        }
    }
}

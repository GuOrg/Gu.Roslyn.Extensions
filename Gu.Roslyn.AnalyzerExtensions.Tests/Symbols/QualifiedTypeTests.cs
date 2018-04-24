namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class QualifiedTypeTests
    {
        [Test]
        public void Equality()
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
            Assert.AreEqual(true, typeSymbol == KnownSymbol.System.Object);
            Assert.AreEqual(false, typeSymbol == KnownSymbol.System.String);
            Assert.AreEqual(false, typeSymbol != new QualifiedType("System.Object"));
            Assert.AreEqual(true, typeSymbol.Is(KnownSymbol.System.Object));
            Assert.AreEqual(false, typeSymbol.Is(KnownSymbol.System.String));
        }
    }
}

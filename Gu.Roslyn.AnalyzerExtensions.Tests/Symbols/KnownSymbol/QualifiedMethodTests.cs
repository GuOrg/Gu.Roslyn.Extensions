namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.KnownSymbol
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class QualifiedMethodTests
    {
        [Test]
        public void SymbolEquality()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class Foo
    {
        internal object Bar()
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(syntaxTree.FindMethodDeclaration("Bar"));
            var qualifiedMethod = new QualifiedMethod(new QualifiedType("N.Foo"), "Bar");
            Assert.AreEqual(true,  symbol == qualifiedMethod);
            Assert.AreEqual(false, symbol != qualifiedMethod);
        }
    }
}

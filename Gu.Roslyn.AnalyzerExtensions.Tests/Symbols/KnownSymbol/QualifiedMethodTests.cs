namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.KnownSymbol
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class QualifiedMethodTests
    {
        [Test]
        public static void SymbolEquality()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class C
    {
        internal object M()
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(syntaxTree.FindMethodDeclaration("M"));
            var qualifiedMethod = new QualifiedMethod(new QualifiedType("N.C"), "M");
            Assert.AreEqual(true, symbol == qualifiedMethod);
            Assert.AreEqual(false, symbol != qualifiedMethod);
        }
    }
}

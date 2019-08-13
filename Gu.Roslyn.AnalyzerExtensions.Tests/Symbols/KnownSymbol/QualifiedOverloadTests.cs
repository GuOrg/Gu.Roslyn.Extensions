namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.KnownSymbol
{
    using System.Collections.Immutable;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class QualifiedOverloadTests
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
            var qualifiedMethod = new QualifiedOverload(new QualifiedType("N.C"), "M", ImmutableArray<QualifiedParameter>.Empty);
            Assert.AreEqual(true, symbol == qualifiedMethod);
            Assert.AreEqual(false, symbol != qualifiedMethod);
        }
    }
}

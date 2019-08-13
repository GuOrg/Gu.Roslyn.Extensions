namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class NamespaceSymbolComparerTests
    {
        [TestCase("N")]
        [TestCase("A.B")]
        [TestCase("A.B.C")]
        public static void Equals(string namespaceName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
}".AssertReplace("N", namespaceName));
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var declaration = syntaxTree.Find<NamespaceDeclarationSyntax>(namespaceName);
            var symbol = semanticModel.GetDeclaredSymbol(declaration, CancellationToken.None);
            Assert.AreEqual(true, NamespaceSymbolComparer.Equals(symbol, namespaceName));
        }
    }
}

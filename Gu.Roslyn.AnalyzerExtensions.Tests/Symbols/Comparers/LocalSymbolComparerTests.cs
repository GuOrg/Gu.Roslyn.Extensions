namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class LocalSymbolComparerTests
    {
        [Test]
        public static void Equals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public C()
        {
            var bar1;
            var bar2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindVariableDeclaration("bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindVariableDeclaration("bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2.Variables[0], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(true, LocalSymbolComparer.Equals((ILocalSymbol)symbol1, (ILocalSymbol)symbol1));
            Assert.AreEqual(false, LocalSymbolComparer.Equals((ILocalSymbol)symbol1, (ILocalSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), LocalSymbolComparer.Default.GetHashCode((ILocalSymbol)symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), LocalSymbolComparer.Default.GetHashCode((ILocalSymbol)symbol2));
        }
    }
}

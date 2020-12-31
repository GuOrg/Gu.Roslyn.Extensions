// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class PropertySymbolComparerTests
    {
        [Test]
        public static void Equal()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public int P1 { get; }
        public int P2 { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindPropertyDeclaration("P1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None);
            var node2 = syntaxTree.FindPropertyDeclaration("P2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, PropertySymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false, PropertySymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), PropertySymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), PropertySymbolComparer.Default.GetHashCode(symbol2));
        }

        [Test]
        public static void Equivalent()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public int P1 { get; }
        public int P2 { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindPropertyDeclaration("P1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None);
            var node2 = syntaxTree.FindPropertyDeclaration("P2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None);
            Assert.AreEqual(true,                                        SymbolComparer.Equivalent((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false,                                       SymbolComparer.Equivalent((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true,                                        PropertySymbolComparer.Equivalent(symbol1, symbol1));
            Assert.AreEqual(false,                                       PropertySymbolComparer.Equivalent(symbol1, symbol2));
        }
    }
}

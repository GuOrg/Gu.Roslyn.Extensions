// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class NamedTypeSymbolComparerTests
    {
        [Test]
        public static void Equals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C1
    {
    }

    public class C2
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindTypeDeclaration("C1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None);
            var node2 = syntaxTree.FindTypeDeclaration("C2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, NamedTypeSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false, NamedTypeSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), NamedTypeSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), NamedTypeSymbolComparer.Default.GetHashCode(symbol2));
        }
    }
}

// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class NamedTypeSymbolComparerTests
    {
        [Test]
        public void Equals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class Foo1
    {
    }

    public class Foo2
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindTypeDeclaration("Foo1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None);
            var node2 = syntaxTree.FindTypeDeclaration("Foo2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, NamedTypeSymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, NamedTypeSymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), NamedTypeSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), NamedTypeSymbolComparer.Default.GetHashCode(symbol2));
        }
    }
}

// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class ParameterSymbolComparerTests
    {
        [Test]
        public void Equals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar(int bar1, int bar2) => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("Bar").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, ParameterSymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, ParameterSymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotSame(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }
    }
}

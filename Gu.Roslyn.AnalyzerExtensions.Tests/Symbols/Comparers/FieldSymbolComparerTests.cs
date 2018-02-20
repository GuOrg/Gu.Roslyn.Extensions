namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class FieldSymbolComparerTests
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
        private int bar1;
        private int bar2;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindBestMatch<FieldDeclarationSyntax>("bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindBestMatch<FieldDeclarationSyntax>("bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2.Declaration.Variables[0], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(true, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol1));
            Assert.AreEqual(false, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol1));
            Assert.AreNotSame(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol2));
        }
    }
}

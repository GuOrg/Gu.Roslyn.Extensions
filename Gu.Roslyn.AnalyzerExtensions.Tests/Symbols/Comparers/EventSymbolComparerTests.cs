namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class EventSymbolComparerTests
    {
        [Test]
        public void Equals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler Bar1;
        public event EventHandler Bar2;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindBestMatch<EventFieldDeclarationSyntax>("Bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindBestMatch<EventFieldDeclarationSyntax>("Bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2.Declaration.Variables[0], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(true, EventSymbolComparer.Equals((IEventSymbol)symbol1, (IEventSymbol)symbol1));
            Assert.AreEqual(false, EventSymbolComparer.Equals((IEventSymbol)symbol1, (IEventSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), EventSymbolComparer.Default.GetHashCode((IEventSymbol)symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), EventSymbolComparer.Default.GetHashCode((IEventSymbol)symbol2));
        }
    }
}

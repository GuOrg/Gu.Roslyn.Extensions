namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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
            var node1 = syntaxTree.FindFieldDeclaration("bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindFieldDeclaration("bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2.Declaration.Variables[0], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(false, SymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(true, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol1));
            Assert.AreEqual(false, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol2));
        }

        [Test]
        public void Inherited()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        protected int Bar;
    }

    public class Bar : Foo
    {
        public Bar()
        {
            var temp = this.Bar;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindFieldDeclaration("Bar");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindMemberAccessExpression("this.Bar");
            var symbol2 = semanticModel.GetSymbolInfo(node2, CancellationToken.None).Symbol;
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol1));
            Assert.AreEqual(true, SymbolComparer.Equals(symbol1, symbol2));
            Assert.AreEqual(true, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol1));
            Assert.AreEqual(true, FieldSymbolComparer.Equals((IFieldSymbol)symbol1, (IFieldSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol2));
        }
    }
}

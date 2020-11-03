namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class FieldSymbolComparerTests
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
        private int f1;
        private int f2;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindFieldDeclaration("f1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindFieldDeclaration("f2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2.Declaration.Variables[0], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false, SymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(true, FieldSymbolComparer.Equal((IFieldSymbol)symbol1, (IFieldSymbol)symbol1));
            Assert.AreEqual(false, FieldSymbolComparer.Equal((IFieldSymbol)symbol1, (IFieldSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol2));
        }

        [Test]
        public static void Inherited()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C1
    {
        protected int F;
    }

    public class C2 : C1
    {
        public C2()
        {
            var temp = this.F;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindFieldDeclaration("F");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1.Declaration.Variables[0], CancellationToken.None);
            var node2 = syntaxTree.FindMemberAccessExpression("this.F");
            var symbol2 = semanticModel.GetSymbolInfo(node2, CancellationToken.None).Symbol;
            Assert.AreEqual(true, SymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(true, SymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(true, FieldSymbolComparer.Equal((IFieldSymbol)symbol1, (IFieldSymbol)symbol1));
            Assert.AreEqual(true, FieldSymbolComparer.Equal((IFieldSymbol)symbol1, (IFieldSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), FieldSymbolComparer.Default.GetHashCode((IFieldSymbol)symbol2));
        }
    }
}

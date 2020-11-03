// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class TypeSymbolComparerTests
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
            Assert.AreEqual(true, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(false, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }

        [Test]
        public static void GenericPropertiesTypeParameterEquals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C<T>
    {
        public T P1 { get; }
        public T P2 { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindPropertyDeclaration("P1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None).Type;
            var node2 = syntaxTree.FindPropertyDeclaration("P2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None).Type;
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(true, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }

        [Test]
        public static void GenericMethodsArgumentTypeParameterEquals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C<T>
    {
        public int M1(T x) => 1;

        public int M2(T x) => 2;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindMethodDeclaration("M1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None).Parameters[0].Type;
            var node2 = syntaxTree.FindMethodDeclaration("M2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None).Parameters[0].Type;
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(true, TypeSymbolComparer.Equal((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }
    }
}

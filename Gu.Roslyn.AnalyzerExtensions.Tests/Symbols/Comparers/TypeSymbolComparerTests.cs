// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class TypeSymbolComparerTests
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
            Assert.AreEqual(true, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(false, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }

        [Test]
        public void GenericPropertiesTypeParameterEquals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class Foo<T>
    {
        public T Bar1 { get; }
        public T Bar2 { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindPropertyDeclaration("Bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None).Type;
            var node2 = syntaxTree.FindPropertyDeclaration("Bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None).Type;
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(true, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }

        [Test]
        public void GenericMethodsArgumentTypeParameterEquals()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class Foo<T>
    {
        public int Bar1(T x) => 1;

        public int Bar2(T x) => 2;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node1 = syntaxTree.FindMethodDeclaration("Bar1");
            var symbol1 = semanticModel.GetDeclaredSymbol(node1, CancellationToken.None).Parameters[0].Type;
            var node2 = syntaxTree.FindMethodDeclaration("Bar2");
            var symbol2 = semanticModel.GetDeclaredSymbol(node2, CancellationToken.None).Parameters[0].Type;
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(true, SymbolComparer.Equals((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol1));
            Assert.AreEqual(true, TypeSymbolComparer.Equals((ITypeSymbol)symbol1, (ITypeSymbol)symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol1));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), TypeSymbolComparer.GetHashCode(symbol2));
        }
    }
}

// ReSharper disable RedundantCast
#pragma warning disable IDE0004
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.Comparers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class ParameterSymbolComparerTests
    {
        [Test]
        public static void Simple()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        int M(int i1, int i2) => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("M").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false, SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true, ParameterSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false, ParameterSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }

        [Test]
        public static void Generic()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C<T>
    {
        int M(T t1, T t2) => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("M").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true,                                        SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false,                                       SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true,                                        ParameterSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false,                                       ParameterSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }

        [Test]
        public static void TwoGenericOutClass()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C<T>
    {
        int M(out T t1, out T t2)
        {
            t1 = default;
            t2 = default;
            return 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("M").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true,                                        SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false,                                       SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true,                                        ParameterSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false,                                       ParameterSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }

        [Test]
        public static void TwoGenericOut()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        int M<T>(out T t1, out T t2)
        {
            t1 = default;
            t2 = default;
            return 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("M").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true,                                        SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false,                                       SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true,                                        ParameterSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false,                                       ParameterSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }

        [Test]
        public static void OneGenericOut()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        int M<T>(int i1, out T t2)
        {
            t2 = default;
            return 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameters = syntaxTree.FindMethodDeclaration("M").ParameterList.Parameters;
            var symbol1 = semanticModel.GetDeclaredSymbol(parameters[0], CancellationToken.None);
            var symbol2 = semanticModel.GetDeclaredSymbol(parameters[1], CancellationToken.None);
            Assert.AreEqual(true,                                        SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol1));
            Assert.AreEqual(false,                                       SymbolComparer.Equal((ISymbol)symbol1, (ISymbol)symbol2));
            Assert.AreEqual(true,                                        ParameterSymbolComparer.Equal(symbol1, symbol1));
            Assert.AreEqual(false,                                       ParameterSymbolComparer.Equal(symbol1, symbol2));
            Assert.AreEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol1));
            Assert.AreNotEqual(SymbolComparer.Default.GetHashCode(symbol1), ParameterSymbolComparer.Default.GetHashCode(symbol2));
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public static class IMethodSymbolExtTests
    {
        [TestCase("new C(1, 2)", "1", "n")]
        [TestCase("new C(1, m: 2)", "1", "n")]
        [TestCase("new C(n: 1, m: 2)", "n: 1", "n")]
        [TestCase("new C(m: 2, n: 1)", "n: 1", "n")]
        [TestCase("new C(1, 2)", "2", "m")]
        [TestCase("new C(1, m: 2)", "m: 2", "m")]
        [TestCase("new C(n: 1, m: 2)", "m: 2", "m")]
        [TestCase("new C(m: 2, n: 1)", "m: 2", "m")]
        public static void TryFindParameter(string objectCreation, string arg, string expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int n, int m)
        {
        }

        public static C M() => new C(1, 2);
    }
}".AssertReplace("new C(1, 2)", objectCreation);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindConstructorDeclaration("C(int n, int m)"), CancellationToken.None, out var method));
            Assert.AreEqual(true, method.TryFindParameter(syntaxTree.FindArgument(arg), out var parameter));
            Assert.AreEqual(expected, parameter.Name);
        }

        [TestCase("new C(1)", "1", "n")]
        [TestCase("new C(1, 2)", "1", "n")]
        [TestCase("new C(1, 2, 3)", "1", "n")]
        [TestCase("new C(1, 2)", "2", "ms")]
        [TestCase("new C(1, 2, 3)", "2", "ms")]
        [TestCase("new C(1, 2, 3)", "3", "ms")]
        public static void TryFindParameterWhenParams(string objectCreation, string arg, string expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int n, params int[] ms)
        {
        }

        public static C M() => new C(1, 2);
    }
}".AssertReplace("new C(1, 2)", objectCreation);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindConstructorDeclaration("C(int n, params int[] ms)"), CancellationToken.None, out var method));
            Assert.AreEqual(true, method.TryFindParameter(syntaxTree.FindArgument(arg), out var parameter));
            Assert.AreEqual(expected, parameter.Name);
        }

        [TestCase("x", true)]
        [TestCase("y", true)]
        [TestCase("missing", false)]
        public static void TryFindParameterExtensionInvocation(string name, bool expected)
        {
            var code = @"
namespace N
{
    public static class C
    {
        public static int Get(int a) => a.M(2);

        public static int M(this int x, int y) => x + y;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true,     semanticModel.TryGetSymbol(syntaxTree.FindInvocation("a.M(2)"), CancellationToken.None, out var method));
            Assert.AreEqual(expected,     method.TryFindParameter(name, out var parameter));
            if (expected)
            {
                Assert.AreEqual(name, parameter.Name);
                Assert.AreEqual(name, method.FindParameter(name).Name);
            }
            else
            {
                Assert.AreEqual(null, method.FindParameter(name));
            }
        }
    }
}

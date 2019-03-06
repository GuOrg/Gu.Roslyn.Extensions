namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class IMethodSymbolExtTests
    {
        [TestCase("new C(1, 2)", "1", "n")]
        [TestCase("new C(1, m: 2)", "1", "n")]
        [TestCase("new C(n: 1, m: 2)", "n: 1", "n")]
        [TestCase("new C(m: 2, n: 1)", "n: 1", "n")]
        [TestCase("new C(1, 2)", "2", "m")]
        [TestCase("new C(1, m: 2)", "m: 2", "m")]
        [TestCase("new C(n: 1, m: 2)", "m: 2", "m")]
        [TestCase("new C(m: 2, n: 1)", "m: 2", "m")]
        public void TryFindParameter(string objectCreation, string arg, string expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class C
    {
        public C(int n, int m)
        {
        }

        public static C M() => new C(1, 2);
    }
}".AssertReplace("new C(1, 2)", objectCreation);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindConstructorDeclaration("C(int n, int m)"), CancellationToken.None, out var method));
            Assert.AreEqual(true, method.TryFindParameter(syntaxTree.FindArgument(arg), out var parameter));
            Assert.AreEqual(expected, parameter.Name);
        }
    }
}

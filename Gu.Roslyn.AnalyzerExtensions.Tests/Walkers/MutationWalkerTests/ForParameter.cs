namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.MutationWalkerTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class ForParameter
    {
        [TestCase("value = 1")]
        [TestCase("value++")]
        [TestCase("value += 1")]
        public static void One(string mutation)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int value)
        {
            value = 1;
        }
    }
}";
            code = code.AssertReplace("value = 1", mutation);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(syntaxTree.Find<ParameterSyntax>("value"));
            using (var walker = MutationWalker.For(symbol, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(mutation, walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual(mutation, single.ToString());
            }
        }
    }
}

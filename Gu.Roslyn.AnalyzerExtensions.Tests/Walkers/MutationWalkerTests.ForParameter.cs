namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public partial class MutationWalkerTests
    {
        public class ForParameter
        {
            [TestCase("value = 1")]
            [TestCase("value++")]
            [TestCase("value += 1")]
            public void Single(string mutation)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int value)
        {
            value = 1;
        }
    }
}";
                testCode = testCode.AssertReplace("value = 1", mutation);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
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
}

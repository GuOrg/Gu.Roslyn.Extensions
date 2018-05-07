namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MutationWalkerTests
    {
        public class ForField
        {
            [TestCase("this.value = 1")]
            [TestCase("this.value++")]
            [TestCase("this.value += 1")]
            public void Single(string mutation)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public Foo()
        {
            this.value = 1;
        }
    }
}";
                testCode = testCode.AssertReplace("this.value = 1", mutation);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(syntaxTree.FindFieldDeclaration("value").Declaration.Variables[0]);
                using (var walker = MutationWalker.For(field, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(mutation, walker.Single().ToString());
                }
            }
        }
    }
}

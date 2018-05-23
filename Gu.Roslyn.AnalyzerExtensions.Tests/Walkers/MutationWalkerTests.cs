namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MutationWalkerTests
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
            var classDeclaration = syntaxTree.FindClassDeclaration("Foo");
            using (var walker = MutationWalker.Borrow(classDeclaration, Scope.Instance, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(mutation, walker.Single().ToString());
            }
        }

        [TestCase("out")]
        [TestCase("ref")]
        public void SingleRefOrOut(string modifier)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public Foo()
        {
            Mutate(out this.value);
        }

        private static void Mutate(out int i)
        {
            i = 1;
        }
    }
}";
            testCode = testCode.AssertReplace("out", modifier);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var classDeclaration = syntaxTree.FindClassDeclaration("Foo");
            using (var walker = MutationWalker.Borrow(classDeclaration, Scope.Instance, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(2, walker.Count);
                Assert.AreEqual($"{modifier} this.value", walker[0].ToString());
                Assert.AreEqual("i = 1", walker[1].ToString());
            }
        }
    }
}

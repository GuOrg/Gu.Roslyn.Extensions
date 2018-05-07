namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
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
            var setter = syntaxTree.FindClassDeclaration("Foo");
            using (var walker = MutationWalker.Borrow(setter))
            {
                Assert.AreEqual(mutation, walker.Single().ToString());
            }
        }

        [TestCase("out this.value")]
        [TestCase("ref this.value")]
        public void SingleRefOut(string arg)
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
            testCode = testCode.AssertReplace("out this.value", arg);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var setter = syntaxTree.FindClassDeclaration("Foo");
            using (var walker = MutationWalker.Borrow(setter))
            {
                Assert.AreEqual(2, walker.Count);
                Assert.AreEqual(arg, walker[0].ToString());
                Assert.AreEqual("i = 1", walker[1].ToString());
            }
        }
    }
}

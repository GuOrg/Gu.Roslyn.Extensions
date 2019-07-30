namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.MutationWalkerTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class WhenOne
    {
        [TestCase("this.value = 1")]
        [TestCase("this.value++")]
        [TestCase("this.value += 1")]
        public static void One(string mutation)
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            this.value = 1;
        }
    }
}";
            code = code.AssertReplace("this.value = 1", mutation);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var classDeclaration = syntaxTree.FindClassDeclaration("C");
            using (var walker = MutationWalker.Borrow(classDeclaration, Scope.Instance, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(mutation, walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual(mutation, single.ToString());
            }
        }

        [TestCase("out")]
        [TestCase("ref")]
        public static void OneRefOrOut(string modifier)
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            Mutate(out this.value);
        }

        private static void Mutate(out int i)
        {
            i = 1;
        }
    }
}";
            code = code.AssertReplace("out", modifier);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var classDeclaration = syntaxTree.FindClassDeclaration("C");
            using (var walker = MutationWalker.Borrow(classDeclaration, Scope.Type, semanticModel, CancellationToken.None))
            {
                CollectionAssert.IsEmpty(walker.PrefixUnaries);
                CollectionAssert.IsEmpty(walker.PostfixUnaries);
                CollectionAssert.AreEqual(new[] { $"{modifier} this.value" }, walker.RefOrOutArguments.Select(x => x.ToString()));
                CollectionAssert.AreEqual(new[] { "i = 1" }, walker.Assignments.Select(x => x.ToString()));
                CollectionAssert.AreEqual(new[] { "i = 1", $"{modifier} this.value" }, walker.All().Select(x => x.ToString()));
            }
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.MutationWalkerTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class ForProperty
    {
        [TestCase("this.Value = 1")]
        [TestCase("this.Value++")]
        [TestCase("this.Value += 1")]
        public static void One(string mutation)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            this.Value = 1;
        }

        public int Value { get; }
    }
}";
            code = code.AssertReplace("this.Value = 1", mutation);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Value"));
            using (var walker = MutationWalker.For(property, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(mutation, walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual(mutation, single.ToString());
            }
        }

        [Test]
        public static void ObjectInitializer()
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value { get; private set; }

        public static C Create() => new C { Value = 1 };
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Value"));
            using (var walker = MutationWalker.For(property, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("Value = 1", walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual("Value = 1", single.ToString());
            }
        }
    }
}

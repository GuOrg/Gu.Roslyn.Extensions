namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MutationWalkerTests
    {
        public class ForProperty
        {
            [TestCase("this.Value = 1")]
            [TestCase("this.Value++")]
            [TestCase("this.Value += 1")]
            public void Single(string mutation)
            {
                var testCode = @"
namespace N
{
    public class Foo
    {
        public Foo()
        {
            this.Value = 1;
        }

        public int Value { get; }
    }
}";
                testCode = testCode.AssertReplace("this.Value = 1", mutation);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
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
            public void ObjectInitializer()
            {
                var testCode = @"
namespace N
{
    public class Foo
    {
        public int Value { get; private set; }

        public static Foo Create() => new Foo { Value = 1 };
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
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
}

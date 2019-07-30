namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.MutationWalkerTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class ForField
    {
        [TestCase("this.value = 1")]
        [TestCase("this.value++")]
        [TestCase("this.value += 1")]
        public void One(string mutation)
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
            var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(syntaxTree.Find<VariableDeclaratorSyntax>("value"));
            using (var walker = MutationWalker.For(field, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(mutation, walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual(mutation, single.ToString());
            }
        }

        [Test]
        public void ObjectInitializer()
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value;

        public static C Create() => new C { value = 1 };
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(syntaxTree.Find<VariableDeclaratorSyntax>("value"));
            using (var walker = MutationWalker.For(field, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("value = 1", walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual("value = 1", single.ToString());
            }
        }

        [Test]
        public void Ref()
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            Update(ref this.value);
        }

        private static void Update(ref int field)
        {
            field++;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(syntaxTree.Find<VariableDeclaratorSyntax>("value"));
            using (var walker = MutationWalker.For(field, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("ref this.value", walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual("ref this.value", single.ToString());
            }
        }

        [Test]
        public void Out()
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            Update(out this.value);
        }

        private static void Update(out int field)
        {
            field = 1;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var field = (IFieldSymbol)semanticModel.GetDeclaredSymbol(syntaxTree.Find<VariableDeclaratorSyntax>("value"));
            using (var walker = MutationWalker.For(field, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("out this.value", walker.All().Single().ToString());
                Assert.AreEqual(true, walker.TrySingle(out var single));
                Assert.AreEqual("out this.value", single.ToString());
            }
        }
    }
}

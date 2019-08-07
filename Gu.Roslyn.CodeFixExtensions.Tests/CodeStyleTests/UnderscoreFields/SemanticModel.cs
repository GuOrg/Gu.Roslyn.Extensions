namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UnderscoreFields
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class SemanticModel
    {
        [Test]
        public static void DefaultsToNull()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
    {
        C(int p)
        {
            this.P = i;
        }

        void M()
        {
        }

        public int P { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(null, CodeStyle.UnderscoreFields(semanticModel));
        }

        [TestCase("private int _f", true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f", false)]
        [TestCase("private readonly int f", false)]
        public static void WhenField(string declaration, bool expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration));

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expected, CodeStyle.UnderscoreFields(semanticModel));
        }

        [TestCase("private int _f",              true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f",               false)]
        [TestCase("private readonly int f",      false)]
        public static void FiguresOutFromOtherTree(string declaration, bool expected)
        {
            var c1 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C1
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration));

            var c2 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C2
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { c1, c2 }, MetadataReferences.FromAttributes());
            Assert.AreEqual(2, compilation.SyntaxTrees.Length);
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                Assert.AreEqual(expected, CodeStyle.UnderscoreFields(semanticModel));
            }
        }

        [Test]
        public static void ChecksContainingClassFirst()
        {
            var c1 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C1
    {
        private int _f;
    }
}");

            var c2 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C1
    {
        private int value;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { c1, c2 }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees[0]);
            Assert.AreEqual(true, CodeStyle.UnderscoreFields(semanticModel));

            semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees[1]);
            Assert.AreEqual(false, CodeStyle.UnderscoreFields(semanticModel));
        }
    }
}

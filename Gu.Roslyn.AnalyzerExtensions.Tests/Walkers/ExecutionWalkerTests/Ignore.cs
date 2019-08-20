namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.ExecutionWalkerTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class Ignore
    {
        [TestCase(SearchScope.Member)]
        [TestCase(SearchScope.Instance)]
        [TestCase(SearchScope.Type)]
        [TestCase(SearchScope.Recursive)]
        public static void IgnoreNameOfProperty(SearchScope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var text = nameof(this.Value);
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(string.Empty, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member)]
        [TestCase(SearchScope.Instance)]
        [TestCase(SearchScope.Type)]
        [TestCase(SearchScope.Recursive)]
        public static void IgnoreNameOfMethod(SearchScope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var text = nameof(this.Value());
        }

        public int Value() => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(string.Empty, string.Join(", ", walker.Literals));
            }
        }
    }
}

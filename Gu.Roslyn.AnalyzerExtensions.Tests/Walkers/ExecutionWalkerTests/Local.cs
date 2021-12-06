namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.ExecutionWalkerTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class Local
    {
        [TestCase(SearchScope.Member, "2")]
        [TestCase(SearchScope.Instance, "1, 2")]
        [TestCase(SearchScope.Type, "1, 2")]
        [TestCase(SearchScope.Recursive, "1, 2")]
        public static void LocalDeclarationWithExpressionBody(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var value = this.Value;
            value = 2;
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
            Assert.AreEqual(expected, string.Join(", ", walker.Literals));
        }

        [TestCase(SearchScope.Member, "2")]
        [TestCase(SearchScope.Instance, "1, 2")]
        [TestCase(SearchScope.Type, "1, 2")]
        [TestCase(SearchScope.Recursive, "1, 2")]
        public static void LocalDeclarationWithCastExpressionBody(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var value = (double)this.Value;
            value = 2;
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
            Assert.AreEqual(expected, string.Join(", ", walker.Literals));
        }
    }
}

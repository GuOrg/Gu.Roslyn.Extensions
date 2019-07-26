namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierTokenWalkerTests
    {
        [Test]
        public void Test()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 1;
        }
    }
}");
            var node = syntaxTree.FindTypeDeclaration("C");
            using (var walker = IdentifierTokenWalker.Borrow(node))
            {
                CollectionAssert.AreEqual(new[] { "C", "C", "var", "i" }, walker.IdentifierTokens.Select(x => x.ValueText));
                Assert.AreEqual(true, walker.TryFind("i", out var match));
                Assert.AreEqual("i", match.ValueText);
                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }
    }
}
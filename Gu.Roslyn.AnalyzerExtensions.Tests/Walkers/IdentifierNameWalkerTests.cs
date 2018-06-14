namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierNameWalkerTests
    {
        [Test]
        public void Test()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int i)
        {
            i = 1;
        }
    }
}");
            var node = syntaxTree.FindTypeDeclaration("Foo");
            using (var walker = IdentifierNameWalker.Borrow(node))
            {
                CollectionAssert.AreEqual(new[] { "i" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));
                Assert.AreEqual(true, walker.TryFind("i", out var match));
                Assert.AreEqual("i", match.Identifier.ValueText);
                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }
    }
}

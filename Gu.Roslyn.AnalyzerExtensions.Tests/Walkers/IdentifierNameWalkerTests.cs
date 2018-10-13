namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierNameWalkerTests
    {
        [Test]
        public void TryFind()
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

        [Test]
        public void TryFindFirst()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int i)
        {
            i = 1;
            i = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindParameter("int i"), CancellationToken.None);
            var node = syntaxTree.FindTypeDeclaration("Foo");

            Assert.AreEqual(true, IdentifierNameWalker.TryFindFirst(node, symbol, semanticModel, CancellationToken.None, out var match));
            Assert.AreEqual("i = 1", match.Parent.ToString());

            using (var walker = IdentifierNameWalker.Borrow(node))
            {
                Assert.AreEqual(true, walker.TryFindFirst(symbol, semanticModel, CancellationToken.None, out match));
                Assert.AreEqual("i = 1", match.Parent.ToString());
            }
        }

        [Test]
        public void TryFindLast()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int i)
        {
            i = 1;
            i = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindParameter("int i"), CancellationToken.None);
            var node = syntaxTree.FindTypeDeclaration("Foo");

            Assert.AreEqual(true, IdentifierNameWalker.TryFindLast(node, symbol, semanticModel, CancellationToken.None, out var match));
            Assert.AreEqual("i = 2", match.Parent.ToString());

            using (var walker = IdentifierNameWalker.Borrow(node))
            {
                Assert.AreEqual(true, walker.TryFindLast(symbol, semanticModel, CancellationToken.None, out match));
                Assert.AreEqual("i = 2", match.Parent.ToString());
            }
        }
    }
}

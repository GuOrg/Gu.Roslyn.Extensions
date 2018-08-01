namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierNameExecutionWalkerTests
    {
        [Test]
        public void Test()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public sealed class Foo
    {
        public static readonly Foo Default = ↓new Foo();
        
        private static readonly string text = ""abc"";
        
        public string Text { get; set; } = text;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindInvocation("Foo");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, Scope.Instance, semanticModel, CancellationToken.None))
            {
                CollectionAssert.AreEqual(new[] { "i" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));
                Assert.AreEqual(true, walker.TryFind("i", out var match));
                Assert.AreEqual("i", match.Identifier.ValueText);
                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class UsagesWalkerTests
    {
        [Test]
        public static void RecursionTargetArgument()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P => M(1);

        public int M(int n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            using var walker = UsagesWalker.Borrow(target.Symbol, target.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("n", string.Join(", ", walker.Usages));
        }

        [Test]
        public static void RecursionTargetArgumentGeneric()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P => this.M<int>(1);

        public T M<T>(T n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            using var walker = UsagesWalker.Borrow(target.Symbol, target.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("n", string.Join(", ", walker.Usages));
        }
    }
}

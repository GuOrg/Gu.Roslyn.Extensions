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
        public static void RecursionTargetExtensionMethodArguments()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public static class C
    {
        public static int P => 1.M(2);

        public static int M(this int m, int n) => m + n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var invocation = tree.FindInvocation("1");
            var invocationTarget = recursion.Target(invocation).Value;
            using var invocationWalker = UsagesWalker.Borrow(invocationTarget.Symbol, invocationTarget.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("m", string.Join(", ", invocationWalker.Usages));

            var argument = tree.FindArgument("2");
            var argumentTarget = recursion.Target(argument).Value;
            using var argumentWalker = UsagesWalker.Borrow(argumentTarget.Symbol, argumentTarget.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("n", string.Join(", ", argumentWalker.Usages));
        }

        [Test]
        public static void RecursionTargetExtensionMethodArgumentsStaticInvocation()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public static class C
    {
        public static int P => M(1, 2);

        public static int M(this int m, int n) => m + n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var argument = tree.FindArgument("1");
            var target = recursion.Target(argument).Value;
            using var walker1 = UsagesWalker.Borrow(target.Symbol, target.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("m", string.Join(", ", walker1.Usages));

            argument = tree.FindArgument("2");
            target = recursion.Target(argument).Value;
            using var walker2 = UsagesWalker.Borrow(target.Symbol, target.TargetNode, semanticModel, CancellationToken.None);
            Assert.AreEqual("n", string.Join(", ", walker2.Usages));
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

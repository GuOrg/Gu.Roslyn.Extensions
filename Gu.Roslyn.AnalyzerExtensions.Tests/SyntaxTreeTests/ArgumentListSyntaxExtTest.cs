namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class ArgumentListSyntaxExtTest
    {
        public class TryFindArgument
        {
            [TestCase(0, "1")]
            [TestCase(1, "2")]
            [TestCase(2, "3")]
            public void Ordinal(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace N
{
    internal class C
    {
        public void M()
        {
            M(1, 2, 3);
        }

        internal void M(int v1, int v2, int v3)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }

            [TestCase(0, "v1: 1")]
            [TestCase(1, "v2: 2")]
            [TestCase(2, "v3: 3")]
            public void NamedAtOrdinalPositions(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace N
{
    internal class C
    {
        public void M()
        {
            M(v1: 1, v2: 2, v3: 3);
        }

        internal void M(int v1, int v2, int v3)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("M(v1: 1, v2: 2, v3: 3)");
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }

            [TestCase(0, "v1: 1")]
            [TestCase(1, "v2: 2")]
            [TestCase(2, "v3: 3")]
            public void Named(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace N
{
    class C
    {
        public void M()
        {
            M(v2: 2, v1: 1, v3: 3);
        }

        internal void M(int v1, int v2, int v3)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("M(v2: 2, v1: 1, v3: 3)");
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }

            [Test]
            public void ParamsExplicitArray()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace N
{
    class C
    {
        public void M()
        {
            M(new[] { 1, 2, 3 });
        }

        internal void M(params int[] xs)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("M(new[] { 1, 2, 3 })");
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[0], out var argument));
                Assert.AreEqual("new[] { 1, 2, 3 }", argument.ToString());

                Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[0], out argument));
                Assert.AreEqual("new[] { 1, 2, 3 }", argument.ToString());
            }

            [Test]
            public void ParamsReturnsFalse()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace N
{
    class C
    {
        public void M()
        {
            M(1, 2, 3);
        }

        internal void M(params int[] xs)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(false,                invocation.TryFindArgument(method.Parameters[0], out var argument));
                Assert.AreEqual(false,                invocation.ArgumentList.TryFind(method.Parameters[0], out argument));
            }
        }
    }
}

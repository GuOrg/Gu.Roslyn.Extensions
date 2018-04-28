namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class ArgumentListSyntaxExtTest
    {
        public class TryGetMatchingArgument
        {
            [TestCase(0, "1")]
            [TestCase(1, "2")]
            [TestCase(2, "3")]
            public void Ordinal(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    namespace RoslynSandbox
    {
        internal class Foo
        {
            public void Bar()
            {
                Meh(1, 2, 3);
            }

            internal void Meh(int v1, int v2, int v3)
            {
            }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("Meh(1, 2, 3)");
                var method = (IMethodSymbol)semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFindArgument(invocation, method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFind(invocation.ArgumentList, method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }

            [TestCase(0, "v1: 1")]
            [TestCase(1, "v2: 2")]
            [TestCase(2, "v3: 3")]
            public void NamedAtOrdinalPositions(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    namespace RoslynSandbox
    {
        internal class Foo
        {
            public void Bar()
            {
                Meh(v1: 1, v2: 2, v3: 3);
            }

            internal void Meh(int v1, int v2, int v3)
            {
            }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("Meh(v1: 1, v2: 2, v3: 3)");
                var method = (IMethodSymbol)semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFindArgument(invocation, method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFind(invocation.ArgumentList, method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }

            [TestCase(0, "v1: 1")]
            [TestCase(1, "v2: 2")]
            [TestCase(2, "v3: 3")]
            public void Named(int index, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
            namespace RoslynSandbox
            {
                namespace RoslynSandbox
                {
                    internal class Foo
                    {
                        public void Bar()
                        {
                            Meh(v2: 2, v1: 1, v3: 3);
                        }

                        internal void Meh(int v1, int v2, int v3)
                        {
                        }
                    }
                }
            }");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation("Meh(v2: 2, v1: 1, v3: 3)");
                var method = (IMethodSymbol)semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFindArgument(invocation, method.Parameters[index], out var argument));
                Assert.AreEqual(expected, argument.ToString());

                Assert.AreEqual(true, ArgumentListSyntaxExt.TryFind(invocation.ArgumentList, method.Parameters[index], out argument));
                Assert.AreEqual(expected, argument.ToString());
            }
        }
    }
}

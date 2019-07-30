namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.ArgumentListSyntaxExtTest
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class TryFindArgumentParams
    {
        [Test]
        public void Ordinal()
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

        internal void M(int i1, params int[] xs)
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
            var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
            Assert.AreEqual(true,   invocation.TryFindArgumentParams(method.Parameters[1], out var arguments));
            Assert.AreEqual("2, 3", string.Join(", ", arguments));

            Assert.AreEqual(true,   invocation.ArgumentList.TryFindParams(method.Parameters[1], out arguments));
            Assert.AreEqual("2, 3", string.Join(", ", arguments));
        }
    }
}

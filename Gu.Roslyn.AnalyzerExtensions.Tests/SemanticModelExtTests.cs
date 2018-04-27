namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class SemanticModelExtTests
    {
        [Test]
        public void TryGetConstantValue()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var value = 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindEqualsValueClause("= 1").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<int>(node, CancellationToken.None, out var value));
            Assert.AreEqual(1, value);
        }
    }
}

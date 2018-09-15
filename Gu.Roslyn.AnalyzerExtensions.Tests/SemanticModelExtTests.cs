namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class SemanticModelExtTests
    {
        [Test]
        public void TryGetConstantValueInt()
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
            var node = syntaxTree.FindEqualsValueClause("=").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<int>(node, CancellationToken.None, out var value));
            Assert.AreEqual(1, value);
            Assert.AreEqual(1, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
        }

        [Test]
        public void TryGetConstantValueString()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var value = ""abc"";
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindEqualsValueClause("=").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<string>(node, CancellationToken.None, out var value));
            Assert.AreEqual("abc", value);
            Assert.AreEqual("abc", semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
        }

        [Test]
        public void TryGetConstantValueStringNull()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            string value = null;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindEqualsValueClause("=").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<string>(node, CancellationToken.None, out var value));
            Assert.AreEqual(null, value);
            Assert.AreEqual(null, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
        }
    }
}

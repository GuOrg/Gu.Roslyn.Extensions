namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System.Reflection;
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
namespace N
{
    public class C
    {
        public C()
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
            Assert.AreEqual(false, semanticModel.TryGetConstantValue<string>(node, CancellationToken.None, out _));
            Assert.AreEqual(false, semanticModel.TryGetConstantValue<double>(node, CancellationToken.None, out _));
        }

        [Test]
        public void TryGetConstantValueDouble()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public C()
        {
            var value = 1.0;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindEqualsValueClause("=").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<double>(node, CancellationToken.None, out var value));
            Assert.AreEqual(1, value);
            Assert.AreEqual(1, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
            Assert.AreEqual(false, semanticModel.TryGetConstantValue<string>(node, CancellationToken.None, out _));
            Assert.AreEqual(false, semanticModel.TryGetConstantValue<int>(node, CancellationToken.None, out _));
        }

        [Test]
        public void TryGetConstantValueString()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public C()
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
namespace N
{
    public class C
    {
        public C()
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

        [Test]
        public void TryGetConstantValueNullableIntNull()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public C()
        {
            int? value = null;
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

        [Test]
        public void TryGetConstantValueBindingFlags()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public C()
        {
            var value = BindingFlags.Instance;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindEqualsValueClause("=").Value;
            Assert.AreEqual(true, semanticModel.TryGetConstantValue<BindingFlags>(node, CancellationToken.None, out var value));
            Assert.AreEqual(BindingFlags.Instance, value);
            Assert.AreEqual(4, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
        }
    }
}

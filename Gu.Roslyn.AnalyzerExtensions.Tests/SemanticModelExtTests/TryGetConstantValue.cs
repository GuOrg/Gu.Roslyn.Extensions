namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests;

using System.Reflection;
using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class TryGetConstantValue
{
    [Test]
    public static void WhenInt()
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
    public static void WhenDouble()
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

    [TestCase("\"abc\"", "abc")]
    [TestCase("null", null)]
    public static void WhenString(string literal, string expected)
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
}".AssertReplace("\"abc\"", literal));
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindEqualsValueClause("=").Value;
        Assert.AreEqual(true, semanticModel.TryGetConstantValue<string>(node, CancellationToken.None, out var value));
        Assert.AreEqual(expected, value);
        Assert.AreEqual(expected, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
    }

    [TestCase("1", 1)]
    [TestCase("null", null)]
    public static void WhenNullableInt(string literal, int? expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    public class C
    {
        public C()
        {
            int? value = 1;
        }
    }
}".AssertReplace("1", literal));
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindEqualsValueClause("=").Value;
        Assert.AreEqual(true, semanticModel.TryGetConstantValue<int?>(node, CancellationToken.None, out var value));
        Assert.AreEqual(expected, value);
        Assert.AreEqual(expected, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
    }

    [Test]
    public static void WhenBindingFlags()
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
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindEqualsValueClause("=").Value;
        Assert.AreEqual(true, semanticModel.TryGetConstantValue<BindingFlags>(node, CancellationToken.None, out var value));
        Assert.AreEqual(BindingFlags.Instance, value);
        Assert.AreEqual(4, semanticModel.GetConstantValueSafe(node, CancellationToken.None).Value);
    }
}

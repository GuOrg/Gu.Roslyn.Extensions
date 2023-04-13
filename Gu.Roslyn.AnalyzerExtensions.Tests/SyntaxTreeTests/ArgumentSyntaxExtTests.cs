namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests;

using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class ArgumentSyntaxExtTests
{
    [TestCase("\"text\"", "text")]
    [TestCase("Const", "const text")]
    [TestCase("string.Empty", "")]
    [TestCase("String.Empty", "")]
    [TestCase("null", null)]
    [TestCase("nameof(C)", "C")]
    [TestCase("nameof(N.C)", "C")]
    [TestCase("nameof(C<int>)", "C")]
    [TestCase("nameof(NestedC<int>)", "NestedC")]
    [TestCase("nameof(M)", "M")]
    [TestCase("nameof(this.M)", "M")]
    [TestCase("(string)null", null)]
    public static void TryGetStringValue(string expression, string expected)
    {
        var code = @"
namespace N
{
    using System;

    public class C
    {
        public const string Const = ""const text"";

        public C()
        {
            M(""text"");
        }

        private void M(string arg) { }

        public class NestedC<T> { }
    }

    public class C<T> { }
}".AssertReplace("\"text\"", expression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation =
        CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindArgument(expression);
        Assert.AreEqual(true, invocation.TryGetStringValue(semanticModel, CancellationToken.None, out var name));
        Assert.AreEqual(expected, name);
    }

    [Test]
    public static void TryGetTypeofValue()
    {
        var code = @"
namespace N
{
    using System;

    public class C
    {
        public C()
        {
            M(typeof(int));
        }

        private void M(Type arg)
        {
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation =
        CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindArgument("typeof(int)");
        Assert.AreEqual(true, invocation.TryGetTypeofValue(semanticModel, CancellationToken.None, out var type));
        Assert.AreEqual("int", type.ToString());
    }
}

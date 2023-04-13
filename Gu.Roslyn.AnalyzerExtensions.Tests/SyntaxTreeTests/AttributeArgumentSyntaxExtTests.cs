namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests;

using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

public static class AttributeArgumentSyntaxExtTests
{
    [TestCase("\"text\"",             "text")]
    [TestCase("Const",                "const text")]
    [TestCase("\"\"",                 "")]
    [TestCase("null",                 null)]
    [TestCase("nameof(C)",            "C")]
    [TestCase("nameof(N.C)",          "C")]
    [TestCase("nameof(C<int>)",       "C")]
    [TestCase("nameof(NestedC<int>)", "NestedC")]
    [TestCase("nameof(M)",            "M")]
    [TestCase("nameof(this.M)",       "M")]
    [TestCase("(string)null",         null)]
    public static void TryGetStringValue(string expression, string expected)
    {
        var code = @"
namespace N
{
    using System;

    [Obsolete(""text"")]
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
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.Find<AttributeSyntax>("Obsolete").ArgumentList.Arguments[0];
        Assert.AreEqual(true,     invocation.TryGetStringValue(semanticModel, CancellationToken.None, out var name));
        Assert.AreEqual(expected, name);
    }
}

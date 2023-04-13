namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests;

using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class SyntaxNodeExtTests
{
    [TestCase("1", true)]
    [TestCase("M", false)]
    public static void WhenReturningExpression(string text, bool expected)
    {
        var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static Expression<Func<int>> M() => () => 1;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.Find<SyntaxNode>(text);
        Assert.AreEqual(expected, node.IsInExpressionTree(semanticModel, CancellationToken.None));
    }

    [TestCase("1", true)]
    [TestCase("M", false)]
    public static void WhenExpressionArgument(string text, bool expected)
    {
        var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static void M() => M(() => 1);

        public static void M(Expression<Func<int>> _) { }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.Find<SyntaxNode>(text);
        Assert.AreEqual(expected, node.IsInExpressionTree(semanticModel, CancellationToken.None));
    }

    [TestCase("1")]
    [TestCase("M")]
    public static void WhenFunc(string text)
    {
        var code = @"
namespace N
{
    using System;

    public class C
    {
        public static Func<int> M() => () => 1;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.Find<SyntaxNode>(text);
        Assert.AreEqual(false, node.IsInExpressionTree(semanticModel, CancellationToken.None));
    }

    [TestCase("1")]
    [TestCase("\abc\".Length")]
    public static void WhenOtherKindArgument(string expression)
    {
        var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static void M() => M(1);

        public static void M(int _) { }
    }
}".AssertReplace("1", expression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.Find<SyntaxNode>(expression);
        Assert.AreEqual(false, node.IsInExpressionTree(semanticModel, CancellationToken.None));
    }
}

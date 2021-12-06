namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.ExpressionSyntaxExtTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class Strings
    {
        [TestCase("\"1\"", "1")]
        [TestCase("@\"1\"", "1")]
        [TestCase("string.Empty", "")]
        [TestCase("String.Empty", "")]
        [TestCase("System.String.Empty", "")]
        [TestCase("nameof(C)", "C")]
        public static void TryGetStringValue(string stringCode, string expected)
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        C()
        {
            var text = string.Empty;
        }
    }
}".AssertReplace("string.Empty", stringCode);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var expression = syntaxTree.FindExpression(stringCode);
            Assert.AreEqual(true, expression.TryGetStringValue(semanticModel, CancellationToken.None, out var stringValue));
            Assert.AreEqual(expected, stringValue);
        }
    }
}

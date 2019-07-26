namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class SyntaxNodeExtTests
    {
        [TestCase("1", true)]
        [TestCase("2", false)]
        [TestCase("M", false)]
        public void IsInExpressionTree(string text, bool expected)
        {
            var testCode = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public Expression<Func<int>> M() => () => 1;
        public Func<int> M() => () => 2;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var expression = syntaxTree.Find<SyntaxNode>(text);
            Assert.AreEqual(expected, expression.IsInExpressionTree(semanticModel, CancellationToken.None));
        }
    }
}

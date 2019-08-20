namespace Gu.Roslyn.AnalyzerExtensions.Tests.ScopeTest
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class ScopeTests
    {
        [TestCase("0", true)]
        [TestCase("1", true)]
        [TestCase("2", true)]
        [TestCase("3", false)]
        [TestCase("4", true)]
        [TestCase("5", false)]
        [TestCase("6", true)]
        [TestCase("7", true)]
        [TestCase("8", true)]
        [TestCase("9", false)]
        public static void IsInStaticContext(string expression, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    [Obsolete(""0"")]
    public class C
    {
        private readonly int f = 1;

        public C()
            :this(2)
        {
        }

        public C(int _)
        {
            this.f = 3;
        }

        public static int P1 => 4;

        public int P2 => 5;

        public static int P3 { get; } = 6;

        public int P4 { get; } = 7;

        public static int M() => 8;

        public int M() => 9;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, node.IsInStaticContext());
        }
}

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

        [TestCase("0",  null,    false)]
        [TestCase("1",  null,    false)]
        [TestCase("2",  null,    false)]
        [TestCase("3",  "x",     true)]
        [TestCase("3",  "WRONG", false)]
        [TestCase("4",  null,    false)]
        [TestCase("5",  "value", true)]
        [TestCase("6",  null,    false)]
        [TestCase("7",  "value", true)]
        [TestCase("8",  "x",     true)]
        [TestCase("9",  "x",     true)]
        [TestCase("10", "x",     true)]
        [TestCase("10", "y",     true)]
        [TestCase("11", "x",     true)]
        [TestCase("11", "z",     true)]
        [TestCase("12", "x",     true)]
        [TestCase("12", "y",     false)]
        [TestCase("13", "x",     true)]
        [TestCase("13", "y",     false)]
        [TestCase("13", "w",     true)]
        public static void HasParameter(string expression, string name, bool expected)
        {
            var code = @"
namespace N
{
    using System;

    [Obsolete(""0"")]
    public class C
    {
        private readonly int f = 1;

        public C()
            :this(2)
        {
        }

        public C(int x)
        {
            this.f = 3;
        }

        public event Action E
        {
            add { _ = 4; }
            remove { _ = 5; }
        }

        public int P
        {
            get => 6;
            set => _ = 7;
        }

        int M(int x) => 8;

        int M(double x)
        {
            return 9;
        }

        static int M(string x)
        {
            Func<int, int> f1 = y => 10;
            Func<int, int> f2 = z => 11;
            return 12;

            int M(long w)
            {
                return 13;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, Scope.HasParameter(node, name));
        }
    }
}

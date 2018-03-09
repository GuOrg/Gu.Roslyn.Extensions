namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.AnalyzerExtensions.SyntaxTree;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    internal class SyntaxNodeExtTests
    {
        internal class IsBeforeInScope
        {
            [TestCase("var temp = 1;", "temp = 2;", true)]
            [TestCase("temp = 2;", "var temp = 1;", false)]
            [TestCase("temp = 1;", "var temp = 1;", false)]
            public void SameBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
internal class Foo
{
    internal Foo()
    {
        var temp = 1;
        temp = 2;
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }

            [TestCase("var temp = 1;", "temp = 2;", true)]
            [TestCase("var temp = 1;", "temp = 3;", true)]
            [TestCase("temp = 2;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "temp = 2;", false)]
            [TestCase("temp = 2;", "temp = 3;", false)]
            public void InsideIfBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            if (true)
            {
                temp = 2;
            }
            else
            {
                temp = 3;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }

            [TestCase("var temp = 1;", "temp = 2;", true)]
            [TestCase("var temp = 1;", "temp = 3;", true)]
            [TestCase("temp = 2;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "temp = 2;", false)]
            [TestCase("temp = 2;", "temp = 3;", false)]
            public void InsideIfBlockCurlyElse(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            if (true)
                temp = 2;
            else
            {
                temp = 3;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }

            [TestCase("var temp = 1;", "temp = 2;", true)]
            [TestCase("var temp = 1;", "temp = 3;", true)]
            [TestCase("var temp = 1;", "temp = 4;", true)]
            [TestCase("temp = 2;", "temp = 4;", true)]
            [TestCase("temp = 3;", "temp = 4;", true)]
            [TestCase("temp = 2;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "var temp = 1;", false)]
            [TestCase("temp = 3;", "temp = 2;", false)]
            [TestCase("temp = 2;", "temp = 3;", false)]
            public void InsideIfBlockNoCurlies(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            if (true)
                temp = 2;
            else
                temp = 3;
            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }

            [TestCase("var temp = 1;", "temp = 4;", true)]
            [TestCase("temp = 2;", "temp = 4;", true)]
            [TestCase("temp = 3;", "temp = 4;", true)]
            [TestCase("temp = 4;", "temp = 2;", false)]
            [TestCase("temp = 4;", "temp = 3;", false)]
            public void AfterIfBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            if (true)
            {
                temp = 2;
            }
            else
            {
                temp = 3;
            }

            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("2", "3", true)]
            [TestCase("3", "2", true)]
            public void Lambda(string firstInt, string otherInt, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo()
        {
            var a = 1;
            this.E += (_, __) => a = 3;
            this.E += (_, __) =>
            {
                a = 4;
                a = 5;
            };
            a = 2;
        }

        public event EventHandler E;
    }
}");
                var first = syntaxTree.FindBestMatch<LiteralExpressionSyntax>(firstInt);
                var other = syntaxTree.FindBestMatch<LiteralExpressionSyntax>(otherInt);
                Assert.AreEqual(expected, first.IsBeforeInScope(other));
            }
        }
    }
}

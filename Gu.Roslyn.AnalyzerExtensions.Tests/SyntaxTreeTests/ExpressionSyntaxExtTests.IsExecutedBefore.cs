namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class ExpressionSyntaxExtTests
    {
        public class IsExecutedBefore
        {
            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            public void And(string firstInt, string otherInt, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public static bool Get(int a, int b) => a == 1 && b == 2;
    }
}");
                var first = syntaxTree.FindLiteralExpression(firstInt);
                var other = syntaxTree.FindLiteralExpression(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            public void Or(string firstInt, string otherInt, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public static bool Get(int a, int b) => a == 1 || b == 2;
    }
}");
                var first = syntaxTree.FindLiteralExpression(firstInt);
                var other = syntaxTree.FindLiteralExpression(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            public void LambdaLocal(string firstInt, string otherInt, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo()
        {
            this.E += (_, __) =>
            {
                var a = 1;
                a = 2;
            };
        }

        public event EventHandler E;
    }
}");
                var first = syntaxTree.FindLiteralExpression(firstInt);
                var other = syntaxTree.FindLiteralExpression(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "3", true)]
            [TestCase("3", "1", false)]
            [TestCase("2", "3", null)]
            [TestCase("3", "2", null)]
            [TestCase("4", "5", true)]
            [TestCase("5", "4", false)]
            [TestCase("3", "4", null)]
            [TestCase("4", "3", null)]
            public void LambdaLocalClosure(string firstInt, string otherInt, bool? expected)
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
                var first = syntaxTree.FindLiteralExpression(firstInt);
                var other = syntaxTree.FindLiteralExpression(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "3", true)]
            [TestCase("3", "1", false)]
            [TestCase("2", "3", null)]
            [TestCase("3", "2", null)]
            [TestCase("4", "5", true)]
            [TestCase("5", "4", false)]
            [TestCase("3", "4", null)]
            [TestCase("4", "3", null)]
            public void LambdaParameterClosure(string firstInt, string otherInt, bool? expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo(int a)
        {
            a = 1;
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
                var first = syntaxTree.FindLiteralExpression(firstInt);
                var other = syntaxTree.FindLiteralExpression(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }
        }
    }
}

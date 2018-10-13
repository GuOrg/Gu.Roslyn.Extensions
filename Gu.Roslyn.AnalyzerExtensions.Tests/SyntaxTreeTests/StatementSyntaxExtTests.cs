namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class StatementSyntaxExtTests
    {
        public class IsExecutedBefore
        {
            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "1", false)]
            public void SameBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 1;
            temp = 2;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "1", false)]
            public void DeclaredInWhileLoop(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            while (true)
            {
                var temp = 1;
                temp = 2;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [Explicit("temp")]
            [TestCase("0", "0", false)]
            [TestCase("1", "1", null)]
            [TestCase("0", "1", true)]
            [TestCase("0", "2", true)]
            [TestCase("1", "2", true)]
            [TestCase("2", "1", null)]
            public void DeclaredBeforeWhileLoop(string firstStatement, string otherStatement, bool? expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var temp = 0;
            while (true)
            {
                temp = 1;
                temp = 2;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "1", false)]
            public void DeclaredInForeachLoop(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int[] ints)
        {
            foreach (var i in ints)
            {
                var temp = 1;
                temp = 2;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "1", false)]
            public void DeclaredInForLoop(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int[] ints)
        {
            for (var index = 0; index < ints.Length; index++)
            {
                var i = ints[index];
                var temp = 1;
                temp = 2;
            }
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("2", "1", false)]
            [TestCase("1", "3", true)]
            [TestCase("3", "1", false)]
            [TestCase("2", "3", false)]
            [TestCase("3", "2", false)]
            public void IfElseBlocks(string firstStatement, string otherStatement, bool expected)
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
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("1", "3", true)]
            [TestCase("2", "1", false)]
            [TestCase("3", "1", false)]
            [TestCase("2", "3", false)]
            [TestCase("3", "2", false)]
            public void IfElseStatements(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(bool condition)
        {
            var temp = 1;
            if (condition)
                temp = 2;
            else
                temp = 3;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("1", "3", true)]
            [TestCase("2", "1", false)]
            [TestCase("3", "1", false)]
            [TestCase("3", "2", false)]
            [TestCase("2", "3", false)]
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
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", true)]
            [TestCase("1", "3", true)]
            [TestCase("1", "4", true)]
            [TestCase("2", "4", true)]
            [TestCase("3", "4", true)]
            [TestCase("2", "1", false)]
            [TestCase("3", "1", false)]
            [TestCase("3", "2", false)]
            [TestCase("2", "3", false)]
            public void InsideIfSingleStatement(string firstStatement, string otherStatement, bool expected)
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
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "4", true)]
            [TestCase("2", "4", true)]
            [TestCase("3", "4", true)]
            [TestCase("4", "2", false)]
            [TestCase("4", "3", false)]
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
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "6", true)]
            [TestCase("1", "2", true)]
            [TestCase("2", "3", true)]
            [TestCase("2", "6", false)]
            [TestCase("1", "5", true)]
            [TestCase("5", "6", false)]
            [TestCase("6", "2", false)]
            [TestCase("4", "5", true)]
            [TestCase("6", "5", false)]
            public void IfReturnBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal void Bar(bool condition)
        {
            var temp = 1;
            if (condition)
            {
                temp = 2;
                temp = 3;
                return;
            }
            else
            {
                temp = 4;
                temp = 5;
                return;
            }

            temp = 6;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "4", null)]
            [TestCase("1", "2", null)]
            [TestCase("2", "4", null)]
            [TestCase("1", "3", null)]
            [TestCase("3", "4", null)]
            [TestCase("4", "2", null)]
            [TestCase("4", "3", null)]
            public void IfReturnBlockWhenGoto(string firstStatement, string otherStatement, bool? expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal void Bar(bool condition)
        {
            var temp = 1;
            meh:
            if (condition)
            {
                temp = 2;
                goto meh;
                return;
            }
            else
            {
                temp = 3;
                goto meh;
                return;
            }

            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "4", true)]
            [TestCase("1", "2", true)]
            [TestCase("2", "4", false)]
            [TestCase("1", "3", true)]
            [TestCase("3", "4", false)]
            [TestCase("4", "2", false)]
            [TestCase("4", "3", false)]
            public void IfThrowBlock(string firstStatement, string otherStatement, bool expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;
	
    internal class Foo
    {
        internal void Bar(bool condition)
        {
            var temp = 1;
            if (condition)
            {
                temp = 2;
                throw new Exception();
            }
            else
            {
                temp = 3;
                throw new Exception();
            }

            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "4", null)]
            [TestCase("1", "2", null)]
            [TestCase("2", "4", null)]
            [TestCase("1", "3", null)]
            [TestCase("3", "4", null)]
            [TestCase("4", "2", null)]
            [TestCase("4", "3", null)]
            public void IfThrowBlockWhenGoto(string firstStatement, string otherStatement, bool? expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;
	
    internal class Foo
    {
        internal void Bar(bool condition)
        {
            var temp = 1;
            meh:
            if (condition)
            {
                temp = 2;
                goto meh;
                throw new Exception();
            }
            else
            {
                temp = 3;
                goto meh;
                throw new Exception();
            }

            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
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
                var first = syntaxTree.FindStatement(firstInt);
                var other = syntaxTree.FindStatement(otherInt);
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
            this.E += (_, __) => 
            {
                a = 3;
            };
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
                var first = syntaxTree.FindStatement(firstInt);
                var other = syntaxTree.FindStatement(otherInt);
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
            this.E += (_, __) => 
            {
                a = 3;
            };
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
                var first = syntaxTree.FindStatement(firstInt);
                var other = syntaxTree.FindStatement(otherInt);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }
        }
    }
}

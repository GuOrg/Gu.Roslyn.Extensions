namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class StatementSyntaxExtTests
    {
        public class IsExecutedBefore
        {
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "1", ExecutedBefore.No)]
            public void SameBlock(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.Maybe)]
            [TestCase("1", "1", ExecutedBefore.Maybe)]
            public void DeclaredInWhileLoop(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("0", "0", ExecutedBefore.No)]
            [TestCase("1", "1", ExecutedBefore.Maybe)]
            [TestCase("0", "1", ExecutedBefore.Yes)]
            [TestCase("0", "2", ExecutedBefore.Yes)]
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.Maybe)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            public void DeclaredBeforeWhileLoop(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            temp = 3;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.Maybe)]
            [TestCase("1", "1", ExecutedBefore.Maybe)]
            public void DeclaredInForeachLoop(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.Maybe)]
            [TestCase("1", "1", ExecutedBefore.Maybe)]
            public void DeclaredInForLoop(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.Maybe)]
            [TestCase("3", "2", ExecutedBefore.No)]
            public void IfBlock(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    internal class Foo
    {
        internal Foo(bool condition)
        {
            var temp = 1;
            if (condition)
            {
                temp = 2;
            }

            temp = 3;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "4", ExecutedBefore.Yes)]
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("2", "4", ExecutedBefore.Maybe)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "2", ExecutedBefore.No)]
            [TestCase("4", "3", ExecutedBefore.No)]
            public void IfElseBlocks(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    internal class Foo
    {
        internal Foo(bool condition)
        {
            var temp = 1;
            if (condition)
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

            [TestCase("1", "4", ExecutedBefore.Yes)]
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("2", "4", ExecutedBefore.Maybe)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "2", ExecutedBefore.No)]
            [TestCase("4", "3", ExecutedBefore.No)]
            public void IfElseStatements(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            temp = 4;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            public void InsideIfBlockCurlyElse(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("1", "4", ExecutedBefore.Yes)]
            [TestCase("2", "4", ExecutedBefore.Maybe)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            public void InsideIfSingleStatement(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "6", ExecutedBefore.Yes)]
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "3", ExecutedBefore.Yes)]
            [TestCase("2", "6", ExecutedBefore.No)]
            [TestCase("1", "5", ExecutedBefore.Yes)]
            [TestCase("5", "6", ExecutedBefore.No)]
            [TestCase("6", "2", ExecutedBefore.No)]
            [TestCase("4", "5", ExecutedBefore.Yes)]
            [TestCase("6", "5", ExecutedBefore.No)]
            public void IfReturnBlock(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "4", ExecutedBefore.Maybe)]
            [TestCase("1", "2", ExecutedBefore.Maybe)]
            [TestCase("2", "4", ExecutedBefore.Maybe)]
            [TestCase("1", "3", ExecutedBefore.Maybe)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "2", ExecutedBefore.Maybe)]
            [TestCase("4", "3", ExecutedBefore.Maybe)]
            public void IfReturnBlockWhenGoto(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "4", ExecutedBefore.Yes)]
            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "4", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "4", ExecutedBefore.No)]
            [TestCase("4", "2", ExecutedBefore.No)]
            [TestCase("4", "3", ExecutedBefore.No)]
            public void IfThrowBlock(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "4", ExecutedBefore.Maybe)]
            [TestCase("1", "2", ExecutedBefore.Maybe)]
            [TestCase("2", "4", ExecutedBefore.Maybe)]
            [TestCase("1", "3", ExecutedBefore.Maybe)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "2", ExecutedBefore.Maybe)]
            [TestCase("4", "3", ExecutedBefore.Maybe)]
            public void IfThrowBlockWhenGoto(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            public void LambdaLocal(string firstInt, string otherInt, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.Maybe)]
            [TestCase("3", "2", ExecutedBefore.Maybe)]
            [TestCase("4", "5", ExecutedBefore.Yes)]
            [TestCase("5", "4", ExecutedBefore.No)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "3", ExecutedBefore.Maybe)]
            public void LambdaLocalClosure(string firstInt, string otherInt, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "3", ExecutedBefore.Yes)]
            [TestCase("3", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.Maybe)]
            [TestCase("3", "2", ExecutedBefore.Maybe)]
            [TestCase("4", "5", ExecutedBefore.Yes)]
            [TestCase("5", "4", ExecutedBefore.No)]
            [TestCase("3", "4", ExecutedBefore.Maybe)]
            [TestCase("4", "3", ExecutedBefore.Maybe)]
            public void LambdaParameterClosure(string firstInt, string otherInt, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
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

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.Yes)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("3", "4", ExecutedBefore.No)]
            [TestCase("3", "6", ExecutedBefore.Maybe)]
            [TestCase("3", "5", ExecutedBefore.Maybe)]
            [TestCase("2", "4", ExecutedBefore.Yes)]
            [TestCase("2", "5", ExecutedBefore.Yes)]
            [TestCase("1", "5", ExecutedBefore.Yes)]
            [TestCase("5", "6", ExecutedBefore.Yes)]
            public void TryCatchCatchFinally(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    internal class Foo
    {
        internal Foo()
        {
            int temp = 1;
            try
            {
                temp = 2;
            }
            catch (FileNotFoundException e)
            {
                temp = 3;
            }
            catch (FileFormatException e)
            {
                temp = 4;
            }
            finally
            {
                temp = 5;
            }

            temp = 6;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("3", "4", ExecutedBefore.No)]
            [TestCase("4", "3", ExecutedBefore.No)]
            [TestCase("2", "4", ExecutedBefore.No)]
            [TestCase("2", "5", ExecutedBefore.Maybe)]
            [TestCase("3", "5", ExecutedBefore.Maybe)]
            [TestCase("4", "5", ExecutedBefore.Maybe)]
            [TestCase("1", "5", ExecutedBefore.Yes)]
            public void Switch(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    internal class Foo
    {
        internal Foo(int i)
        {
            int temp = 1;
            switch (i)
            {
                case 0:
                    temp = 2;
                    break;
                case 1:
                    temp = 3;
                    break;
                default:
                    temp = 4;
                    break;
            }

            temp = 5;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }

            [TestCase("1", "2", ExecutedBefore.Yes)]
            [TestCase("2", "1", ExecutedBefore.No)]
            [TestCase("1", "1", ExecutedBefore.No)]
            [TestCase("2", "3", ExecutedBefore.No)]
            [TestCase("3", "2", ExecutedBefore.No)]
            [TestCase("3", "4", ExecutedBefore.No)]
            [TestCase("4", "3", ExecutedBefore.No)]
            [TestCase("2", "4", ExecutedBefore.No)]
            [TestCase("2", "5", ExecutedBefore.Maybe)]
            [TestCase("3", "5", ExecutedBefore.Maybe)]
            [TestCase("4", "5", ExecutedBefore.Maybe)]
            [TestCase("1", "5", ExecutedBefore.Yes)]
            public void SwitchPattern(string firstStatement, string otherStatement, ExecutedBefore expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    internal class Foo
    {
        internal Foo(int i)
        {
            int temp = 1;
            switch (i)
            {
                case int n when n == 0:
                    temp = 2;
                    break;
                case int n when n == 1:
                    temp = 3;
                    break;
                default:
                    temp = 4;
                    break;
            }

            temp = 5;
        }
    }
}");
                var first = syntaxTree.FindStatement(firstStatement);
                var other = syntaxTree.FindStatement(otherStatement);
                Assert.AreEqual(expected, first.IsExecutedBefore(other));
            }
        }
    }
}

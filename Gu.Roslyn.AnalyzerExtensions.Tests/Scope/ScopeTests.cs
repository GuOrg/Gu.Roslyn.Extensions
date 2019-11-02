namespace Gu.Roslyn.AnalyzerExtensions.Tests.Scope
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;
    using Scope = Gu.Roslyn.AnalyzerExtensions.Scope;

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
        [TestCase("10", true)]
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

        [Obsolete(""10"")]
        public int M() => 9;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, node.IsInStaticContext());
        }

        [TestCase("0", null, false)]
        [TestCase("1", null, false)]
        [TestCase("2", null, false)]
        [TestCase("3", "x", true)]
        [TestCase("3", "WRONG", false)]
        [TestCase("4", null, false)]
        [TestCase("5", "value", true)]
        [TestCase("6", null, false)]
        [TestCase("7", "value", true)]
        [TestCase("8", "x", true)]
        [TestCase("9", "x", true)]
        [TestCase("10", "x", true)]
        public static void HasParameter(string expression, string name, bool expected)
        {
            var code = @"
namespace N
{
    using System;

    [Obsolete(""0"")]
    class C
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
            return 10;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, Scope.HasParameter(node, name));
        }

        [TestCase("1", "x", "Func<int, int> f = y => 2", true)]
        [TestCase("1", "y", "Func<int, int> f = y => 2", false)]
        [TestCase("2", "x", "Func<int, int> f = y => 2", true)]
        [TestCase("2", "y", "Func<int, int> f = y => 2", true)]
        [TestCase("3", "x", "Func<int, int> f = y => 2", true)]
        [TestCase("3", "y", "Func<int, int> f = y => 2", false)]
        [TestCase("1", "x", "Func<int, int> f = (y) => 2", true)]
        [TestCase("1", "y", "Func<int, int> f = (y) => 2", false)]
        [TestCase("2", "x", "Func<int, int> f = (y) => 2", true)]
        [TestCase("2", "y", "Func<int, int> f = (y) => 2", true)]
        [TestCase("3", "x", "Func<int, int> f = (y) => 2", true)]
        [TestCase("3", "y", "Func<int, int> f = (y) => 2", false)]
        public static void HasParameterLambda(string location, string name, string lambda, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(string x)
        {
            x = 1;
            Func<int, int> f = y => 2;
            return 3;
        }
    }
}".AssertReplace("Func<int, int> f = y => 2", lambda);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasParameter(node, name));
        }

        [TestCase("1", "x", true)]
        [TestCase("1", "y", false)]
        [TestCase("2", "x", true)]
        [TestCase("2", "y", true)]
        public static void HasParameterLocalFunction(string location, string name, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(string x)
        {
            return 1;

            int M(long y)
            {
                return 2;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasParameter(node, name));
        }

        [TestCase("1", "x", true)]
        [TestCase("2", "x", true)]
        public static void HasParameterLocalNested(string location, string name, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(bool x)
        {
            if (x)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasParameter(node, name));
        }

        [TestCase("0", null, false)]
        [TestCase("1", null, false)]
        [TestCase("2", null, false)]
        [TestCase("3", "x", true)]
        [TestCase("3", "WRONG", false)]
        [TestCase("4", null, false)]
        [TestCase("5", null, false)]
        [TestCase("6", null, false)]
        [TestCase("7", null, false)]
        [TestCase("8", null, false)]
        [TestCase("9", "x", true)]
        [TestCase("10", "x", true)]
        [TestCase("10", "y", false)]
        [TestCase("10", "z", false)]
        [TestCase("11", "x", true)]
        [TestCase("11", "y", true)]
        public static void HasLocal(string expression, string name, bool expected)
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

        public C()
        {
            var x = 'a';
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

        int M1() => 8;

        int M2()
        {
            var x = 'a';
            return 9;
        }

        static int M3(bool b)
        {
            var x = 'a';
            return 10;

            if (text is null)
            {
                var y = 'a';
                return 11;
            }

            int M(long _)
            {
                var z = 'a';
                return 12;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, Scope.HasLocal(node, name));
        }

        [TestCase("1", "x", true)]
        [TestCase("1", "y", false)]
        [TestCase("1", "z", false)]
        [TestCase("2", "x", true)]
        [TestCase("2", "y", true)]
        [TestCase("2", "z", false)]
        [TestCase("3", "x", true)]
        [TestCase("3", "y", false)]
        [TestCase("3", "z", true)]
        public static void HasLocalLocalNested(string location, string name, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(bool b)
        {
            var x = b;
            return 1;

            if (x)
            {
                var y = b;
                return 2;
            }
            else
            {
                var z = b;
                return 3;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasLocal(node, name));
        }

        [TestCase("1", "x", "out var y", true)]
        [TestCase("1", "y", "out var y", true)]
        [TestCase("2", "x", "out var y", true)]
        [TestCase("2", "y", "out var y", true)]
        [TestCase("3", "x", "out var y", true)]
        [TestCase("3", "y", "out var y", true)]
        [TestCase("1", "x", "out int y", true)]
        [TestCase("1", "y", "out int y", true)]
        [TestCase("2", "x", "out int y", true)]
        [TestCase("2", "y", "out int y", true)]
        [TestCase("3", "x", "out int y", true)]
        [TestCase("3", "y", "out int y", true)]
        public static void HasLocalLocalOut(string location, string name, string text, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(string text)
        {
            var x = 1;

            if (int.TryParse(out var y))
            {
                return 2;
            }

            return 3;
        }
    }
}".AssertReplace("out var y", text);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasLocal(node, name));
        }

        [TestCase("1", "x", true)]
        [TestCase("1", "y", true)]
        [TestCase("2", "x", true)]
        [TestCase("2", "y", true)]
        [TestCase("3", "x", true)]
        [TestCase("3", "y", true)]
        public static void HasLocalLocalIsPattern(string location, string name, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(object o)
        {
            var x = 1;

            if (o is int y)
            {
                return 2;
            }

            return 3;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasLocal(node, name));
        }

        [TestCase("1", "x", true)]
        [TestCase("1", "y", false)]
        [TestCase("2", "x", true)]
        [TestCase("2", "y", true)]
        [TestCase("3", "x", true)]
        [TestCase("3", "y", false)]
        public static void HasLocalSwitch(string location, string name, bool expected)
        {
            var code = @"
namespace N
{
    class C
    {
        int M(object o)
        {
            var x = 1;
            switch (o)
            {
                case int y:
                    return 2;
            }

            return 3;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(location);
            Assert.AreEqual(expected, Scope.HasLocal(node, name));
        }
    }
}

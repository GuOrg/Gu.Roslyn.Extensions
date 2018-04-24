namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public partial class EnumerableExtTests
    {
        [Test]
        public void TryFirstParameter()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }

        internal void Bar()
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirst(out var parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirstOfType(out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirstOfType(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "d", out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "missing", out parameter));

            var barDeclaration = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(false, barDeclaration.ParameterList.Parameters.TryFirst(out parameter));
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "missing", out parameter));
        }

        [Test]
        public void TryLastParameter()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }

        internal void Bar()
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLast(out var parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLastOfType(out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLast(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLastOfType(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLast(x => x.Identifier.ValueText == "d", out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryLastOfType(x => x.Identifier.ValueText == "d", out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryLast(x => x.Identifier.ValueText == "missing", out parameter));

            var barDeclaration = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(false, barDeclaration.ParameterList.Parameters.TryFirst(out parameter));
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryLast(x => x.Identifier.ValueText == "missing", out parameter));
        }

        [Test]
        public void TrySingleParameter()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }

        internal void Bar()
        {
        }

        internal void Baz(int i)
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TrySingle(out var parameter));

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TrySingle(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TrySingleOfType(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TrySingle(x => x.Identifier.ValueText == "d", out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TrySingleOfType(x => x.Identifier.ValueText == "d", out parameter));
            Assert.AreEqual("double d", parameter.ToString());

            Assert.AreEqual(false, ctor.ParameterList.Parameters.TrySingle(x => x.Identifier.ValueText == "missing", out parameter));

            var barDeclaration = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(false, barDeclaration.ParameterList.Parameters.TrySingle(out parameter));
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TrySingle(x => x.Identifier.ValueText == "missing", out parameter));

            var bazDeclaration = syntaxTree.FindMethodDeclaration("Baz");
            Assert.AreEqual(true, bazDeclaration.ParameterList.Parameters.TrySingle(out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(true, bazDeclaration.ParameterList.Parameters.TrySingleOfType(out parameter));
            Assert.AreEqual("int i", parameter.ToString());
        }

        [Test]
        public void TrySingleMethod()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal int Bar() => 1;

        internal int Bar(int i) => i;
    }
}");
            var type = syntaxTree.FindClassDeclaration("Foo");
            Assert.AreEqual(false, type.Members.TrySingle(out var member));

            Assert.AreEqual(false, type.Members.TrySingle(x => x is MethodDeclarationSyntax methodDeclaration && methodDeclaration.Identifier.ValueText == "Bar", out member));

            Assert.AreEqual(true, type.Members.TrySingle(x => x is MethodDeclarationSyntax methodDeclaration && methodDeclaration.ParameterList.Parameters.Count == 0, out member));
            Assert.AreEqual("internal int Bar() => 1;", member.ToString());

            Assert.AreEqual(true, type.Members.TrySingle(x => x is MethodDeclarationSyntax methodDeclaration && methodDeclaration.ParameterList.Parameters.Count == 1, out member));
            Assert.AreEqual("internal int Bar(int i) => i;", member.ToString());
        }

        [TestCase(0, "int i")]
        [TestCase(1, "double d")]
        public void TryElementAtParameter(int index, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryElementAt(index, out var result));
            Assert.AreEqual(expected, result.ToString());
        }

        [TestCase(5)]
        public void TryElementAtParameterWhenOutOfBounds(int index)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryElementAt(index, out _));
        }

        [Test]
        public void TryElementAtParameterWhenEmpty()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
        }
    }
}");
            var ctor = syntaxTree.FindConstructorDeclaration("Foo");
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryElementAt(0, out _));
        }
    }
}

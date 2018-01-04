namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
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

            Assert.AreEqual(true, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "i", out parameter));
            Assert.AreEqual("int i", parameter.ToString());

            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "missing", out parameter));

            var barDeclaration = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(false, barDeclaration.ParameterList.Parameters.TryFirst(out parameter));
            Assert.AreEqual(false, ctor.ParameterList.Parameters.TryFirst(x => x.Identifier.ValueText == "missing", out parameter));
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

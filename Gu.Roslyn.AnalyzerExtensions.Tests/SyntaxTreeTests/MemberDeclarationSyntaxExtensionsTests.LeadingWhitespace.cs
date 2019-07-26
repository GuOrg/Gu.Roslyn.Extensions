namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MemberDeclarationSyntaxExtensionsTests
    {
        public class LeadingWhitespace
        {
            [Test]
            public void Class()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
    }
}");
                var node = syntaxTree.FindClassDeclaration("Foo");
                CodeAssert.AreEqual("    ", node.LeadingWhitespace());
            }

            [Test]
            public void ClassWithPragmaAndDocs()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
    /// <summary>
    /// The Foo
    /// </summary>
    public class Foo
    {
    }
}");
                var node = syntaxTree.FindClassDeclaration("Foo");
                CodeAssert.AreEqual("    ", node.LeadingWhitespace());
            }

            [Test]
            public void ClassWithDocs()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    /// <summary>
    /// The Foo
    /// </summary>
    public class Foo
    {
    }
}");
                var node = syntaxTree.FindClassDeclaration("Foo");
                CodeAssert.AreEqual("    ", node.LeadingWhitespace());
            }

            [Test]
            public void Constructor()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary> Initializes a new instance of the <see cref=""Foo""/> class. </summary>
        public Foo()
        {
        }
    }
}");
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                CodeAssert.AreEqual("        ", node.LeadingWhitespace());
            }

            [Test]
            public void ConstructorWithDocs()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        public Foo()
        {
        }
    }
}");
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                CodeAssert.AreEqual("        ", node.LeadingWhitespace());
            }

            [Test]
            public void Property()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public int Value { get; set; }
    }
}");
                var node = syntaxTree.FindPropertyDeclaration("Value");
                CodeAssert.AreEqual("        ", node.LeadingWhitespace());
            }

            [Test]
            public void Method()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        public int Id(int i) => i;
    }
}");
                var node = syntaxTree.FindMethodDeclaration("Id");
                CodeAssert.AreEqual("        ", node.LeadingWhitespace());
            }

            [Test]
            public void MethodWithDocs()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public int Id(int i) => i;
    }
}");
                var node = syntaxTree.FindMethodDeclaration("Id");
                CodeAssert.AreEqual("        ", node.LeadingWhitespace());
            }
        }
    }
}

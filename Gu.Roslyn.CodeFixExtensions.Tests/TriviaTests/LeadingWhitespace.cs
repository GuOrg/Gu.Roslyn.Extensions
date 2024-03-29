namespace Gu.Roslyn.CodeFixExtensions.Tests.TriviaTests;

using Gu.Roslyn.Asserts;
using Gu.Roslyn.CodeFixExtensions;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class LeadingWhitespace
{
    [Test]
    public static void Class()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
    }
}");
        var node = syntaxTree.FindClassDeclaration("C");
        CodeAssert.AreEqual("    ", node.LeadingWhitespace());
    }

    [Test]
    public static void ClassWithPragmaAndDocs()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
    /// <summary>
    /// The C
    /// </summary>
    public class C
    {
    }
}");
        var node = syntaxTree.FindClassDeclaration("C");
        CodeAssert.AreEqual("    ", node.LeadingWhitespace());
    }

    [Test]
    public static void ClassWithDocs()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    /// <summary>
    /// The C
    /// </summary>
    public class C
    {
    }
}");
        var node = syntaxTree.FindClassDeclaration("C");
        CodeAssert.AreEqual("    ", node.LeadingWhitespace());
    }

    [Test]
    public static void Constructor()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary> Initializes a new instance of the <see cref=""C""/> class. </summary>
        public C()
        {
        }
    }
}");
        var node = syntaxTree.FindConstructorDeclaration("C");
        CodeAssert.AreEqual("        ", node.LeadingWhitespace());
    }

    [Test]
    public static void ConstructorWithDocs()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
        }
    }
}");
        var node = syntaxTree.FindConstructorDeclaration("C");
        CodeAssert.AreEqual("        ", node.LeadingWhitespace());
    }

    [Test]
    public static void Property()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
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
    public static void Method()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int Id(int i) => i;
    }
}");
        var node = syntaxTree.FindMethodDeclaration("Id");
        CodeAssert.AreEqual("        ", node.LeadingWhitespace());
    }

    [Test]
    public static void MethodWithDocs()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
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

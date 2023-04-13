namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.MemberDeclarationSyntaxExtensionsTests;

using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class TryGetDocumentationComment
{
    [Test]
    public static void Class()
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
        var classDeclaration = syntaxTree.FindClassDeclaration("C");
        Assert.AreEqual(true, classDeclaration.TryGetDocumentationComment(out var result));
        var expected = "/// <summary>\r\n    /// The C\r\n    /// </summary>\r\n";
        CodeAssert.AreEqual(expected, result.ToFullString());
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
        var ctor = syntaxTree.FindConstructorDeclaration("C");
        Assert.AreEqual(true, ctor.TryGetDocumentationComment(out var result));
        var expected = "/// <summary> Initializes a new instance of the <see cref=\"C\"/> class. </summary>\r\n";
        CodeAssert.AreEqual(expected, result.ToFullString());
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
        var property = syntaxTree.FindPropertyDeclaration("Value");
        Assert.AreEqual(true, property.TryGetDocumentationComment(out var result));
        var expected = "/// <summary>\r\n" +
                       "        /// Gets or sets the value\r\n" +
                       "        /// </summary>\r\n";
        CodeAssert.AreEqual(expected, result.ToFullString());
    }

    [Test]
    public static void Method()
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
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var result));
        var expected = "/// <summary>\r\n" +
                       "        /// The identity function.\r\n" +
                       "        /// </summary>\r\n" +
                       "        /// <param name=\"i\">The value to return.</param>\r\n" +
                       "        /// <returns><paramref name=\"i\"/></returns>\r\n";
        CodeAssert.AreEqual(expected, result.ToFullString());
    }

    [Test]
    public static void ClassWithPragma()
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
        var classDeclaration = syntaxTree.FindClassDeclaration("C");
        Assert.AreEqual(true, classDeclaration.TryGetDocumentationComment(out var result));
        var expected = "/// <summary>\r\n    /// The C\r\n    /// </summary>\r\n";
        CodeAssert.AreEqual(expected, result.ToFullString());
    }
}

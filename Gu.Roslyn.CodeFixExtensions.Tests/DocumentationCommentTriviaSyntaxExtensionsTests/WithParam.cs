namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentationCommentTriviaSyntaxExtensionsTests;

using Gu.Roslyn.AnalyzerExtensions;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

public static class WithParam
{
    [Test]
    public static void WhenSummaryAndReturn()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenSummaryOnlySingleLine()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>The identity function.</summary>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>The identity function.</summary>
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenSummaryOnlyMultiLine()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenReturnsOnlySingleLine()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenReturnsOnlyMultiLine()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <returns>
        /// The value passed in.
        /// </returns>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        /// <returns>
        /// The value passed in.
        /// </returns>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenParamExistsInsertAfter()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The first <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The first <see cref=""int""/> term.</param>
        /// <param name=""y"">The other <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Add");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("y", "The other <see cref=\"int\"/> term.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"y\">The other <see cref=\"int\"/> term.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void WhenParamExistsInsertBefore()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""y"">The other <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The first <see cref=""int""/> term.</param>
        /// <param name=""y"">The other <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Add");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        var updated = comment.WithParamText("x", "The first <see cref=\"int\"/> term.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"x\">The first <see cref=\"int\"/> term.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    [Test]
    public static void ReplaceExisting()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <param name=""i"">Old</param>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
        var method = syntaxTree.FindMethodDeclaration("Id");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));

        var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
        RoslynAssert.Ast(expected, updated);

        updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
        RoslynAssert.Ast(expected, updated);
    }

    private static DocumentationCommentTriviaSyntax GetExpected(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var method = syntaxTree.FindMethodDeclaration("(");
        Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
        return comment;
    }
}

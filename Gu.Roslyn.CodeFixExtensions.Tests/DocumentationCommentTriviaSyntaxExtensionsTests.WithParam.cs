namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        public class WithParam
        {
            [Test]
            public void WhenSummaryAndReturn()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenSummaryOnlySingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>The identity function.</summary>
        public int Id(int i) => i;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>The identity function.</summary>
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        public int Id(int i) => i;
    }
}");
                var method = syntaxTree.FindMethodDeclaration("Id");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenSummaryOnlyMultiLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        public int Id(int i) => i;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenReturnsOnlySingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <param name=""i"">The <see cref=""int""/> to return.</param>
        /// <returns>The value passed in.</returns>
        public int Id(int i) => i;
    }
}");
                var method = syntaxTree.FindMethodDeclaration("Id");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithParamText("i", "The <see cref=\"int\"/> to return.");
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenReturnsOnlyMultiLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <returns>
        /// The value passed in.
        /// </returns>
        public int Id(int i) => i;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenParamExistsInsertAfter()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The first <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"y\">The other <see cref=\"int\"/> term.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void WhenParamExistsInsertBefore()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""y"">The other <see cref=""int""/> term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
                var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"x\">The first <see cref=\"int\"/> term.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceExisting()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
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
namespace RoslynSandbox
{
    public class Foo
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
                AnalyzerAssert.Ast(expected, updated);

                updated = comment.WithParam(Parse.XmlElementSyntax("<param name=\"i\">The <see cref=\"int\"/> to return.</param>", "        "));
                AnalyzerAssert.Ast(expected, updated);
            }
        }
    }
}

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
            public void WhenSummaryOnly()
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
            public void WhenReturnsOnly()
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

namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        [Test]
        public void WithSingleLineSummary()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithSummary("New text.");
            Assert.AreEqual(true, updated.TryGetSummary(out var summary));
            Assert.AreEqual("<summary> New text. </summary>", summary.ToFullString());

            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary> New text. </summary>
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
            AssertAst(expected, updated);
        }

        [Test]
        public void WithMultiLineSummary()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithSummary("New\r\ntext.");
            Assert.AreEqual(true, updated.TryGetSummary(out var summary));
            Assert.AreEqual("<summary> New\r\n text.\r\n </summary>", summary.ToFullString());

            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>
        /// New
        /// text.
        /// </summary>
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
            AssertAst(expected, updated);
        }

        [Test]
        public void WithSingleLineSummaryReplaceOldSingleLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary> The identity function. </summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Id");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithSummary("New text.");
            Assert.AreEqual(true, updated.TryGetSummary(out var summary));
            Assert.AreEqual("<summary> New text. </summary>", summary.ToFullString());

            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary> New text. </summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
            AssertAst(expected, updated);
        }

        [Test]
        public void WithSingleLineSummaryReplaceOldMultiLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>
        /// The identity function.
        /// </summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Id");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithSummary("New text.");
            Assert.AreEqual(true, updated.TryGetSummary(out var summary));
            Assert.AreEqual("<summary> New text. </summary>", summary.ToFullString());

            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary> New text. </summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
            AssertAst(expected, updated);
        }

        private static void AssertAst(DocumentationCommentTriviaSyntax expected, DocumentationCommentTriviaSyntax actual)
        {
            var expectedAst = Dump.Ast(expected);
            var actualAst = Dump.Ast(actual);
            CodeAssert.AreEqual(expectedAst, actualAst);
        }

        private static DocumentationCommentTriviaSyntax GetExpected(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var method = syntaxTree.FindMethodDeclaration("(");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            return comment;
        }
    }
}

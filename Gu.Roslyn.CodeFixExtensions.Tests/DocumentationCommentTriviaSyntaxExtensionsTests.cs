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
            var updated = comment.WithSummaryText("New text.");
            Assert.AreEqual(true, updated.TryGetSummary(out var summary));

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
            AnalyzerAssert.Ast(expected, updated);

            updated = comment.WithSummary(Parse.XmlElementSyntax("<summary> New text. </summary>", "        "));
            Assert.AreEqual(true, updated.TryGetSummary(out summary));
            AnalyzerAssert.Ast(expected, updated);
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
            var updated = comment.WithSummaryText("Line 1.\r\nLine 2.");
            Assert.AreEqual(true, updated.TryGetSummary(out _));

            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>
        /// Line 1.
        /// Line 2.
        /// </summary>
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
            AnalyzerAssert.Ast(expected, updated);
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
            var updated = comment.WithSummaryText("New text.");
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
            AnalyzerAssert.Ast(expected, updated);
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
            var updated = comment.WithSummaryText("New text.");
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
            AnalyzerAssert.Ast(expected, updated);
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

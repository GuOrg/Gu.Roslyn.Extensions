namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentationCommentTriviaSyntaxExtensionsTests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class WithReturns
    {
        [Test]
        public static void InsertAfterRemarksSingleLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <remarks></remarks>
        /// <returns>New text.</returns>
        public void M()
        {
        }
    }
}");
            var method = syntaxTree.FindMethodDeclaration("M");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithReturnsText("New text.");
            RoslynAssert.Ast(expected, updated);

            updated = comment.WithReturns(Parse.XmlElementSyntax("<returns>New text.</returns>", "        "));
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public static void InsertAfterRemarksMultiLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <remarks></remarks>
        /// <returns>
        /// Line 1.
        /// Line 2.
        /// </returns>
        public void M()
        {
        }
    }
}");
            var method = syntaxTree.FindMethodDeclaration("M");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithReturnsText("Line 1.\r\nLine 2.");
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public static void ReplaceSingleLineWithSingleLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>The identity function.</summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns>Old text.</returns>
        public T Id<T>(T i) => i;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>The identity function.</summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Id");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithReturnsText("<paramref name=\"i\"/>");
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
}

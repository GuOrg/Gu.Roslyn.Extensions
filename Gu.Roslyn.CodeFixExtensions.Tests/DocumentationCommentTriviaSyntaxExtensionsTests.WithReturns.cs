namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        public class WithReturns
        {
            [Test]
            public void InsertAfterRemarksSingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
                var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <remarks></remarks>
        /// <returns>New text.</returns>
        public void Bar()
        {
        }
    }
}");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithReturnsText("New text.");
                RoslynAssert.Ast(expected, updated);

                updated = comment.WithReturns(Parse.XmlElementSyntax("<returns>New text.</returns>", "        "));
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void InsertAfterRemarksMultiLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <remarks></remarks>
        public void Bar()
        {
        }
    }
}");
                var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <remarks></remarks>
        /// <returns>
        /// Line 1.
        /// Line 2.
        /// </returns>
        public void Bar()
        {
        }
    }
}");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithReturnsText("Line 1.\r\nLine 2.");
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceSingleLineWithSingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
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
    public class Foo
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
        }
    }
}

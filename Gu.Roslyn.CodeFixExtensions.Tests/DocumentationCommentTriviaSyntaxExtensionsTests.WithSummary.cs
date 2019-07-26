namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        public class WithSummary
        {
            [Test]
            public void InsertBeforeRemarksSingleLineSummary()
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
                var method = syntaxTree.FindMethodDeclaration("M");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithSummaryText("New text.");
                Assert.AreEqual(true, updated.TryGetSummary(out _));

                var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>New text.</summary>
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
                RoslynAssert.Ast(expected, updated);

                updated = comment.WithSummary(Parse.XmlElementSyntax("<summary>New text.</summary>", "        "));
                Assert.AreEqual(true, updated.TryGetSummary(out _));
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void InsertBeforeRemarksSingleLineSummaryWhenPragma()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
                var method = syntaxTree.FindMethodDeclaration("M");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithSummaryText("New text.");
                Assert.AreEqual(true, updated.TryGetSummary(out _));

                var expected = GetExpected(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>New text.</summary>
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
                RoslynAssert.Ast(expected, updated);

                updated = comment.WithSummary(Parse.XmlElementSyntax("<summary>New text.</summary>", "        "));
                Assert.AreEqual(true, updated.TryGetSummary(out _));
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void InsertBeforeRemarksMultiLineSummary()
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
                var method = syntaxTree.FindMethodDeclaration("M");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithSummaryText("Line 1.\r\nLine 2.");
                Assert.AreEqual(true, updated.TryGetSummary(out _));

                var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>
        /// Line 1.
        /// Line 2.
        /// </summary>
        /// <remarks></remarks>
        public void M()
        {
        }
    }
}");
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceSingleLineWithSingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
                var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>New text.</summary>
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
                Assert.AreEqual("<summary>New text.</summary>", summary.ToFullString());
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceWhenPragma()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>Old text.</summary>
        /// <typeparam name=""T"">The type</typeparam>
        /// <param name=""i"">The value to return.</param>
        /// <returns><paramref name=""i""/></returns>
        public T Id<T>(T i) => i;
    }
}");
                var expected = GetExpected(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>New text.</summary>
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
                Assert.AreEqual("<summary>New text.</summary>", summary.ToFullString());
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceMultiLineWithSingleLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
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
                var expected = GetExpected(@"
namespace N
{
    public class C
    {
        /// <summary>New text.</summary>
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
                Assert.AreEqual("<summary>New text.</summary>", summary.ToFullString());
                RoslynAssert.Ast(expected, updated);

                updated = comment.WithSummary(Parse.XmlElementSyntax("<summary>New text.</summary>", "        "));
                RoslynAssert.Ast(expected, updated);
            }
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Text;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class DocumentationCommentTriviaSyntaxExtensionsTests
    {
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
            var expectedAst = DumpAst(expected);
            var actualAst = DumpAst(actual);
            CodeAssert.AreEqual(expectedAst, actualAst);
        }

        private static DocumentationCommentTriviaSyntax GetExpected(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var method = syntaxTree.FindMethodDeclaration("Id");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            return comment;
        }

        private static string DumpAst(SyntaxNode node)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{node.Kind()} {node.ToString().Replace("\r", "\\r").Replace("\n", "\\n")}");
            foreach (var child in node.DescendantNodesAndTokens())
            {
                DumpAst(child, builder);
            }

            return builder.ToString();
        }

        private static void DumpAst(SyntaxNodeOrToken nodeOrToken, StringBuilder builder)
        {
            builder.AppendLine($"{nodeOrToken.Kind()} {nodeOrToken.ToString().Replace("\r", "\\r").Replace("\n", "\\n")}");
            foreach (var child in nodeOrToken.ChildNodesAndTokens())
            {
                DumpAst(child, builder);
            }
        }
    }
}

namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static partial class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        public static class WithTypeParam
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
        /// <param name=""x"">The value.</param>
        /// <returns>The value passed in.</returns>
        public T Id<T>(T x) => x;
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
        /// <typeparam name=""T"">The type of the value.</typeparam>
        /// <param name=""x"">The value.</param>
        /// <returns>The value passed in.</returns>
        public T Id<T>(T x) => x;
    }
}");
                var method = syntaxTree.FindMethodDeclaration("Id");
                Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
                var updated = comment.WithTypeParamText("T", "The type of the value.");
                RoslynAssert.Ast(expected, updated);

                updated = comment.WithTypeParam(Parse.XmlElementSyntax("<typeparam name=\"T\">The type of the value.</typeparam>", "        "));
                RoslynAssert.Ast(expected, updated);
            }
        }
    }
}

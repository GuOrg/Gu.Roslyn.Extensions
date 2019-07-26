namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public partial class DocumentationCommentTriviaSyntaxExtensionsTests
    {
        [Test]
        public void InsertBeforeFirst()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <returns>One</returns>
        public int Bar() => 1;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>Bar</summary>
        /// <returns>One</returns>
        public int Bar() => 1;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var element = Parse.XmlElementSyntax("<summary>Bar</summary>", "        ");
            var updated = comment.InsertBefore(comment.Content.OfType<XmlElementSyntax>().First(), element);
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public void InsertBeforeSecond()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""y"">The y.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The x.</param>
        /// <param name=""y"">The y.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Add");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var element = Parse.XmlElementSyntax("<param name=\"x\">The x.</param>", "        ");
            var updated = comment.InsertBefore(comment.Content.OfType<XmlElementSyntax>().Skip(1).First(), element);
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public void InsertAfterFirst()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>Bar</summary>
        public int Bar() => 1;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>Bar</summary>
        /// <returns>One</returns>
        public int Bar() => 1;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var element = Parse.XmlElementSyntax("<returns>One</returns>", "        ");
            var updated = comment.InsertAfter(comment.Content.OfType<XmlElementSyntax>().First(), element);
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public void InsertAfterSecond()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The x.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The x.</param>
        /// <param name=""y"">The y.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Add");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var element = Parse.XmlElementSyntax("<param name=\"y\">The y.</param>", "        ");
            var updated = comment.InsertAfter(comment.Content.OfType<XmlElementSyntax>().Skip(1).First(), element);
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public void InsertAfterLast()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>The id function.</summary>
        /// <param name=""x"">The x.</param>
        public int Id(int x) => x;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>The id function.</summary>
        /// <param name=""x"">The x.</param>
        /// <returns>The value passed in</returns>
        public int Id(int x) => x;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Id");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            Assert.AreEqual(true, comment.Content.TrySingleOfType(x => x.HasLocalName("param"), out XmlElementSyntax param));
            var element = Parse.XmlElementSyntax("<returns>The value passed in</returns>", "        ");
            var updated = comment.InsertAfter(param, element);
            RoslynAssert.Ast(expected, updated);
        }

        [Test]
        public void WithSummaryTextParamAndReturns()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        public int Add(int x, int y) => x + y;
    }
}");
            var expected = GetExpected(@"
namespace N
{
    public class Foo
    {
        /// <summary>Sum two numbers.</summary>
        /// <param name=""x"">The first term.</param>
        /// <param name=""y"">The other term.</param>
        /// <returns>The sum.</returns>
        public int Add(int x, int y) => x + y;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Add");
            Assert.AreEqual(true, method.TryGetDocumentationComment(out var comment));
            var updated = comment.WithParamText("x", "The first term.")
                                 .WithParamText("y", "The other term.")
                                 .WithReturnsText("The sum.");
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

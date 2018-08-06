namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class MemberDeclarationSyntaxExtensionsTests
    {
        [Test]
        public void Add()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar() => 1;
    }
}");
            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var docs = "        /// <summary>New summary.</summary>\r\n" +
                       "        /// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs, adjustLeadingWhitespace: false);
            AnalyzerAssert.Ast(expected, updated);
        }

        [Test]
        public void AddAdjustWhiteSpace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar() => 1;
    }
}");
            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var docs = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs, adjustLeadingWhitespace: true);
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
        /// <summary>Old summary.</summary>
        /// <returns>Old returns.</returns>
        public int Bar() => 1;
    }
}");
            var expected = GetExpected(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var docs = "        /// <summary>New summary.</summary>\r\n" +
                       "        /// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs);
            AnalyzerAssert.Ast(expected, updated);
        }

        private static MethodDeclarationSyntax GetExpected(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            return syntaxTree.FindMethodDeclaration("(");
        }
    }
}

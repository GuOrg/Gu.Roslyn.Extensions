namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
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
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var docs = "        /// <summary>New summary.</summary>\r\n" +
                       "        /// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs, adjustLeadingWhitespace: false);
            AnalyzerAssert.Ast(expected, updated);
        }

        [Test]
        public void AddToSecondMember()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar1() => 1;

        public int Bar2() => 2;
    }
}");
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar1() => 1;

        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar2() => 2;
    }
}").FindMethodDeclaration("Bar2");
            var method = syntaxTree.FindMethodDeclaration("Bar2");
            var docs = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs);
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
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var text = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(text, adjustLeadingWhitespace: true);
            AnalyzerAssert.Ast(expected, updated);
        }

        [Test]
        public void AddAdjustWhiteSpaceDefaultHandleMissingNewLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar() => 1;
    }
}");
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var text = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(text);
            AnalyzerAssert.Ast(expected, updated);
        }

        [Test]
        public void AddAdjustWhiteSpaceDefault()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar() => 1;
    }
}");
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var text = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>\r\n";
            var updated = method.WithDocumentationText(text);
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
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
            var method = syntaxTree.FindMethodDeclaration("Bar");
            var docs = "        /// <summary>New summary.</summary>\r\n" +
                       "        /// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs);
            AnalyzerAssert.Ast(expected, updated);
        }

        [Test]
        public void ReplaceExistingSecondMember()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar1() => 1;

        /// <summary>Old summary.</summary>
        /// <returns>Old returns.</returns>
        public int Bar2() => 2;
    }
}");
            var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar1() => 1;

        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int Bar2() => 2;
    }
}").FindMethodDeclaration("Bar2");
            var method = syntaxTree.FindMethodDeclaration("Bar2");
            var docs = "/// <summary>New summary.</summary>\r\n" +
                       "/// <returns>New returns.</returns>";
            var updated = method.WithDocumentationText(docs);
            AnalyzerAssert.Ast(expected, updated);
        }
    }
}

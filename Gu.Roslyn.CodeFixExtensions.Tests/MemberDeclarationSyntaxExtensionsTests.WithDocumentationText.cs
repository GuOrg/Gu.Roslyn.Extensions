namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MemberDeclarationSyntaxExtensionsTests
    {
        public class WithDocumentationText
        {
            [Test]
            public void Add()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var docs = "        /// <summary>New summary.</summary>\r\n" +
                           "        /// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhenPragma()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var docs = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhenPragmaExplicitWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var docs = "        /// <summary>New summary.</summary>\r\n" +
                           "        /// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddToSecondMember()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M1() => 1;

        public int M2() => 2;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M1() => 1;

        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M2() => 2;
    }
}").FindMethodDeclaration("M2");
                var method = syntaxTree.FindMethodDeclaration("M2");
                var docs = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddAdjustWhiteSpace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var text = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(text, adjustLeadingWhitespace: true);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddAdjustWhiteSpaceDefaultHandleMissingNewLine()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var text = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(text);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddAdjustWhiteSpaceDefault()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var text = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>\r\n";
                var updated = method.WithDocumentationText(text);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceExisting()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>Old summary.</summary>
        /// <returns>Old returns.</returns>
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var docs = "        /// <summary>New summary.</summary>\r\n" +
                           "        /// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceExistingWhenPragma()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>Old summary.</summary>
        /// <returns>Old returns.</returns>
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var docs = "        /// <summary>New summary.</summary>\r\n" +
                           "        /// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void ReplaceExistingSecondMember()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M1() => 1;

        /// <summary>Old summary.</summary>
        /// <returns>Old returns.</returns>
        public int M2() => 2;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int M1() => 1;

        /// <summary>New summary.</summary>
        /// <returns>New returns.</returns>
        public int M2() => 2;
    }
}").FindMethodDeclaration("M2");
                var method = syntaxTree.FindMethodDeclaration("M2");
                var docs = "/// <summary>New summary.</summary>\r\n" +
                           "/// <returns>New returns.</returns>";
                var updated = method.WithDocumentationText(docs);
                RoslynAssert.Ast(expected, updated);
            }
        }
    }
}

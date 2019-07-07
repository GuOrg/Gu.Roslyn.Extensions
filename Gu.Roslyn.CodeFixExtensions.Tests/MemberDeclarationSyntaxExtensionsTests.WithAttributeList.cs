namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MemberDeclarationSyntaxExtensionsTests
    {
        public class WithAttributeList
        {
            [Test]
            public void AddDontAdjustWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public int Bar() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        [Obsolete]
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                var attribute = "        [Obsolete]";
                var updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public int Bar() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        [Obsolete]
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                var attribute = "[Obsolete]";
                var updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: true);
                RoslynAssert.Ast(expected, updated);

                updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: true);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhenPragma()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int Bar() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        [Obsolete]
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                var attribute = "[Obsolete]";
                var updated = method.WithAttributeListText(attribute);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhenPragmaExplicitWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int Bar() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        [Obsolete]
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int Bar() => 1;
    }
}").FindMethodDeclaration("Bar");
                var method = syntaxTree.FindMethodDeclaration("Bar");
                var attribute = "        [Obsolete]";
                var updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }
        }
    }
}

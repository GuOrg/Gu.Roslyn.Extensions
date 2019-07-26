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
namespace N
{
    using System;

    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
        [Obsolete]
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var attribute = "        [Obsolete]";
                var updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
        [Obsolete]
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
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
namespace N
{
    using System;

    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
        [Obsolete]
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var attribute = "[Obsolete]";
                var updated = method.WithAttributeListText(attribute);
                RoslynAssert.Ast(expected, updated);
            }

            [Test]
            public void AddWhenPragmaExplicitWhitespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}");
                var expected = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C
    {
        [Obsolete]
#pragma warning disable WPF0013 // CLR accessor for attached property must match registered type.
        public int M() => 1;
    }
}").FindMethodDeclaration("M");
                var method = syntaxTree.FindMethodDeclaration("M");
                var attribute = "        [Obsolete]";
                var updated = method.WithAttributeListText(attribute, adjustLeadingWhitespace: false);
                RoslynAssert.Ast(expected, updated);
            }
        }
    }
}

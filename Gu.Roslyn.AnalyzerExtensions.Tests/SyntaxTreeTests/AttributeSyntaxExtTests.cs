using System.Runtime.CompilerServices;

namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class AttributeSyntaxExtTests
    {
        [TestCase("[Obsolete(\"abc\")]")]
        [TestCase("[Obsolete(message: \"abc\")]")]
        public static void TrySingleArgument(string attributeText)
        {
            string code = @"
namespace N
{
    using System;

    [Obsolete(""abc"")]
    public class C
    {
    }
}".AssertReplace("[Obsolete(\"abc\")]", attributeText);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(true, attribute.TrySingleArgument(out var argument));
            Assert.AreEqual("\"abc\"", argument.Expression.ToString());
        }

        [TestCase("[Obsolete(\"abc\")]")]
        [TestCase("[Obsolete(message: \"abc\")]")]
        public static void TryFindFirstArgumentNameColon(string attributeText)
        {
            string code = @"
namespace N
{
    using System;

    [Obsolete(""abc"")]
    public class C
    {
    }
}".AssertReplace("[Obsolete(\"abc\")]", attributeText);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(true, attribute.TryFindArgument(0, "message", out var argument));
            Assert.AreEqual("\"abc\"", argument.Expression.ToString());
        }

        [TestCase("[Obsolete(\"abc\", true)]")]
        [TestCase("[Obsolete(message: \"abc\", error: true)]")]
        public static void TryFindSecondArgumentNameColon(string attributeText)
        {
            string code = @"
namespace N
{
    using System;

    [Obsolete(""abc"", true)]
    public class C
    {
    }
}".AssertReplace("[Obsolete(\"abc\", true)]", attributeText);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(true,      attribute.TryFindArgument(1, "error", out var argument));
            Assert.AreEqual("true", argument.Expression.ToString());
        }

        [TestCase("[assembly:InternalsVisibleTo(\"abc\", AllInternalsVisible = true)]")]
        public static void TryFindSecondArgumentNameEquals(string attributeText)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(attributeText);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(true,   attribute.TryFindArgument(0, "assemblyName", out var argument));
            Assert.AreEqual("\"abc\"", argument.Expression.ToString());
            Assert.AreEqual(true,   attribute.TryFindArgument(1, "AllInternalsVisible", out argument));
            Assert.AreEqual("true", argument.Expression.ToString());
        }

        [TestCase("[Obsolete(\"abc\")]")]
        [TestCase("[Obsolete(message: \"abc\")]")]
        public static void TryFindArgumentNameColonWhenMissing(string attributeText)
        {
            string code = @"
namespace N
{
    using System;

    [Obsolete(""abc"")]
    public class C
    {
    }
}".AssertReplace("[Obsolete(\"abc\")]", attributeText);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(false,      attribute.TryFindArgument(1, "error", out _));
        }

        [TestCase("[assembly:InternalsVisibleTo(\"abc\")]")]
        public static void TryFindSecondArgumentNameEqualsWhenMissing(string attributeText)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(attributeText);
            var attribute = syntaxTree.FindAttribute(attributeText);
            Assert.AreEqual(true,      attribute.TryFindArgument(0, "assemblyName", out var argument));
            Assert.AreEqual("\"abc\"", argument.Expression.ToString());
            Assert.AreEqual(false,   attribute.TryFindArgument(1, "AllInternalsVisible", out _));
        }
    }
}

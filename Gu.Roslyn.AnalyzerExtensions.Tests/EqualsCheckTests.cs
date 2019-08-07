namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class EqualsCheckTests
    {
        [TestCase("text == null")]
        [TestCase("text != null")]
        [TestCase("Equals(text, null)")]
        [TestCase("object.Equals(text, null)")]
        [TestCase("Object.Equals(text, null)")]
        [TestCase("ReferenceEquals(text, null)")]
        [TestCase("object.ReferenceEquals(text, null)")]
        [TestCase("Object.ReferenceEquals(text, null)")]
        public void IsEqualsCheck(string check)
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        bool M(string text) => text == null;
    }
}".AssertReplace("text == null", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.Find<ExpressionSyntax>(check);
            Assert.AreEqual(true,   EqualsCheck.IsEqualsCheck(expression, out var left, out var right));
            Assert.AreEqual("text", left.ToString());
            Assert.AreEqual("null", right.ToString());
        }

        [TestCase("Equals(text, null)",                 true)]
        [TestCase("object.Equals(text, null)",          true)]
        [TestCase("Object.Equals(text, null)",          true)]
        [TestCase("ReferenceEquals(text, null)",        false)]
        [TestCase("object.ReferenceEquals(text, null)", false)]
        [TestCase("Object.ReferenceEquals(text, null)", false)]
        public void IsObjectEquals(string check, bool expected)
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        bool M(string text) => Equals(text, null);
    }
}".AssertReplace("Equals(text, null)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation(check);
            Assert.AreEqual(expected, EqualsCheck.IsObjectEquals(expression, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }

        [TestCase("Equals(text, null)",                 false)]
        [TestCase("object.Equals(text, null)",          false)]
        [TestCase("Object.Equals(text, null)",          false)]
        [TestCase("ReferenceEquals(text, null)",        true)]
        [TestCase("object.ReferenceEquals(text, null)", true)]
        [TestCase("Object.ReferenceEquals(text, null)", true)]
        public void IsObjectReferenceEquals(string check, bool expected)
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        bool M(string text) => Equals(text, null);
    }
}".AssertReplace("Equals(text, null)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation(check);
            Assert.AreEqual(expected, EqualsCheck.IsObjectReferenceEquals(expression, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }
    }
}

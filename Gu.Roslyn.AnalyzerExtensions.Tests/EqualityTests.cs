namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class EqualityTests
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
            Assert.AreEqual(true,   Equality.IsEqualsCheck(expression, default, default, out var left, out var right));
            Assert.AreEqual("text", left.ToString());
            Assert.AreEqual("null", right.ToString());

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true,   Equality.IsEqualsCheck(expression, semanticModel, CancellationToken.None, out left, out right));
            Assert.AreEqual("text", left.ToString());
            Assert.AreEqual("null", right.ToString());
        }

        [TestCase("Equals(null, text)",                    "null",    "text",    true,  true)]
        [TestCase("Equals(text, null)",                    "text",    "null",    true,  true)]
        [TestCase("object.Equals(text, null)",             "text",    "null",    true,  true)]
        [TestCase("Object.Equals(text, null)",             "text",    "null",    true,  true)]
        [TestCase("System.Object.Equals(text, null)",      "text",    "null",    true,  true)]
        [TestCase("Equals(text, MISSING)",                 "text",    "MISSING", true,  false)]
        [TestCase("object.Equals(text, MISSING)",          "text",    "MISSING", true,  false)]
        [TestCase("Object.Equals(text, MISSING)",          "text",    "MISSING", true,  false)]
        [TestCase("Nullable.Equals(text, null)",           "text",    "null",    true,  true)]
        [TestCase("Nullable.Equals(1, null)",              "1",       "null",    true,  true)]
        [TestCase("Nullable.Equals(1, 1)",                 "1",       "1",       true,  true)]
        [TestCase("Nullable.Equals((int?)1, 1)",           "(int?)1", "1",       true,  false)]
        [TestCase("System.Nullable.Equals(text, null)",    "text",    "null",    true,  true)]
        [TestCase("System.Nullable.Equals(text, MISSING)", "text",    "MISSING", true,  false)]
        [TestCase("object.ReferenceEquals(text, null)",    null,      null,      false, false)]
        [TestCase("Object.ReferenceEquals(text, null)",    null,      null,      false, false)]
        [TestCase("text.Equals(null)",                     "text",    "null",    false, false)]
        public void IsObjectEquals(string check, string expectedLeft, string expectedRight, bool syntaxExpected, bool symbolExpected)
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
            Assert.AreEqual(syntaxExpected, Equality.IsObjectEquals(expression, default, default, out var left, out var right));
            if (syntaxExpected)
            {
                Assert.AreEqual(expectedLeft,  left.ToString());
                Assert.AreEqual(expectedRight, right.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(symbolExpected, Equality.IsObjectEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (symbolExpected)
            {
                Assert.AreEqual(expectedLeft,  left.ToString());
                Assert.AreEqual(expectedRight, right.ToString());
            }
        }

        [TestCase("text.Equals(null)",         "text", "null", true,  true)]
        [TestCase("text?.Equals(null)",        "text", "null", true,  true)]
        [TestCase("Equals(text, null)",        null,   null,   false, false)]
        [TestCase("object.Equals(text, null)", null,   null,   false, false)]
        public void IsInstanceEquals(string check, string expectedLeft, string expectedRight, bool syntaxExpected, bool symbolExpected)
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        bool M(string text) => text.Equals(null);
    }
}".AssertReplace("text.Equals(null)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation(check);
            Assert.AreEqual(syntaxExpected, Equality.IsInstanceEquals(expression, default, default, out var left, out var right));
            if (syntaxExpected)
            {
                Assert.AreEqual(expectedLeft,  left.ToString());
                Assert.AreEqual(expectedRight, right.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(symbolExpected, Equality.IsInstanceEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (symbolExpected)
            {
                Assert.AreEqual(expectedLeft,  left.ToString());
                Assert.AreEqual(expectedRight, right.ToString());
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
            Assert.AreEqual(expected, Equality.IsObjectReferenceEquals(expression, default, default, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expected, Equality.IsObjectReferenceEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }

        [TestCase("text == null", true)]
        [TestCase("text != null", false)]
        public void IsOperatorEquals(string check, bool expected)
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
            var expression = syntaxTree.FindBinaryExpression(check);
            Assert.AreEqual(expected, Equality.IsOperatorEquals(expression, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }

        [TestCase("text == null", false)]
        [TestCase("text != null", true)]
        public void IsOperatorNotEquals(string check, bool expected)
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
            var expression = syntaxTree.FindBinaryExpression(check);
            Assert.AreEqual(expected, Equality.IsOperatorNotEquals(expression, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }
    }
}

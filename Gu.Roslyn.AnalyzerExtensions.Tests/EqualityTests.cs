namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class EqualityTests
    {
        [TestCase("M(string text) => text == null",                                                      "text", "null")]
        [TestCase("M(string text) => text != null",                                                      "text", "null")]
        [TestCase("M(string text) => Equals(text, null)",                                                "text", "null")]
        [TestCase("M(string text) => text.Equals(null)",                                                 "text", "null")]
        [TestCase("M(string text) => text?.Equals(null)",                                                "text", "null")]
        [TestCase("M(string text) => object.Equals(text, null)",                                         "text", "null")]
        [TestCase("M(string text) => Object.Equals(text, null)",                                         "text", "null")]
        [TestCase("M(string text) => System.Object.Equals(text, null)",                                  "text", "null")]
        [TestCase("M(string text) => ReferenceEquals(text, null)",                                       "text", "null")]
        [TestCase("M(string text) => object.ReferenceEquals(text, null)",                                "text", "null")]
        [TestCase("M(string text) => Object.ReferenceEquals(text, null)",                                "text", "null")]
        [TestCase("M(string text) => RuntimeHelpers.Equals(text, null)",                                 "text", "null")]
        [TestCase("M(string text) => System.Runtime.CompilerServices.RuntimeHelpers.Equals(text, null)", "text", "null")]
        [TestCase("M(int? x, int? y) => Nullable.Equals(x, y)",                                          "x",    "y")]
        [TestCase("M(string x, string y) => StringComparer.Ordinal.Equals(x, y)",                        "x",    "y")]
        public void IsEqualsCheck(string check, string expectedLeft, string expectedRight)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

    class C
    {
        bool M(string text) => text == null;
    }
}".AssertReplace("M(string text) => text == null", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.Find<ExpressionSyntax>(check.Split('>').Last().TrimStart());
            Assert.AreEqual(true,          Equality.IsEqualsCheck(expression, default, default, out var left, out var right));
            Assert.AreEqual(expectedLeft,  left.ToString());
            Assert.AreEqual(expectedRight, right.ToString());

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true,          Equality.IsEqualsCheck(expression, semanticModel, CancellationToken.None, out left, out right));
            Assert.AreEqual(expectedLeft,  left.ToString());
            Assert.AreEqual(expectedRight, right.ToString());
        }

        [TestCase("Equals(null, text)",                                                "null",    "text",    true,  true)]
        [TestCase("Equals(text, null)",                                                "text",    "null",    true,  true)]
        [TestCase("object.Equals(text, null)",                                         "text",    "null",    true,  true)]
        [TestCase("Object.Equals(text, null)",                                         "text",    "null",    true,  true)]
        [TestCase("System.Object.Equals(text, null)",                                  "text",    "null",    true,  true)]
        [TestCase("Equals(text, MISSING)",                                             "text",    "MISSING", true,  false)]
        [TestCase("object.Equals(text, MISSING)",                                      "text",    "MISSING", true,  false)]
        [TestCase("Object.Equals(text, MISSING)",                                      "text",    "MISSING", true,  false)]
        [TestCase("Nullable.Equals(text, null)",                                       "text",    "null",    true,  true)]
        [TestCase("Nullable.Equals(1, null)",                                          "1",       "null",    true,  true)]
        [TestCase("Nullable.Equals(1, 1)",                                             "1",       "1",       true,  true)]
        [TestCase("Nullable.Equals((int?)1, 1)",                                       "(int?)1", "1",       true,  false)]
        [TestCase("System.Nullable.Equals(text, null)",                                "text",    "null",    true,  true)]
        [TestCase("System.Nullable.Equals(text, MISSING)",                             "text",    "MISSING", true,  false)]
        [TestCase("object.ReferenceEquals(text, null)",                                null,      null,      false, false)]
        [TestCase("Object.ReferenceEquals(text, null)",                                null,      null,      false, false)]
        [TestCase("System.Object.ReferenceEquals(text, null)",                         null,      null,      false, false)]
        [TestCase("RuntimeHelpers.Equals(text, null)",                                 null,      null,      false, false)]
        [TestCase("System.Runtime.CompilerServices.RuntimeHelpers.Equals(text, null)", null,      null,      false, false)]
        [TestCase("text.Equals(null)",                                                 "text",    "null",    false, false)]
        public void IsObjectEquals(string check, string expectedLeft, string expectedRight, bool syntaxExpected, bool symbolExpected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

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

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
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

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
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
    using System.Runtime.CompilerServices;

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

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expected, Equality.IsObjectReferenceEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }

        [TestCase("RuntimeHelpers.Equals(text, null)",                                 true)]
        [TestCase("System.Runtime.CompilerServices.RuntimeHelpers.Equals(text, null)", true)]
        [TestCase("object.Equals(text, null)",                                         false)]
        [TestCase("Object.Equals(text, null)",                                         false)]
        [TestCase("System.Object.Equals(text, null)",                                  false)]
        [TestCase("ReferenceEquals(text, null)",                                       false)]
        [TestCase("object.ReferenceEquals(text, null)",                                false)]
        [TestCase("Object.ReferenceEquals(text, null)",                                false)]
        public void IsRuntimeHelpersEquals(string check, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

    class C
    {
        bool M(string text) => RuntimeHelpers.Equals(text, null);
    }
}".AssertReplace("RuntimeHelpers.Equals(text, null)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation(check);
            Assert.AreEqual(expected, Equality.IsRuntimeHelpersEquals(expression, default, default, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expected, Equality.IsRuntimeHelpersEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (expected)
            {
                Assert.AreEqual("text", left.ToString());
                Assert.AreEqual("null", right.ToString());
            }
        }

        [TestCase("M(int? x, int? y) => Nullable.Equals(x, y)",        true,  true)]
        [TestCase("M(int? x, int? y) => System.Nullable.Equals(x, y)", true,  true)]
        [TestCase("M(int? x, int y) => Nullable.Equals(x, y)",         true,  true)]
        [TestCase("M(int? x, string y) => Nullable.Equals(x, y)",      true,  false)]
        [TestCase("M(int? x, int? y) => object.Equals(x, y)",          false, false)]
        [TestCase("M(int? x, int? y) => Object.Equals(x, y)",          false, false)]
        public void IsNullableEquals(string check, bool expectedSyntax, bool expectedSymbol)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

    class C
    {
        bool M(int? x, int? y) => Nullable.Equals(x, y);
    }
}".AssertReplace("M(int? x, int? y) => Nullable.Equals(x, y)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation("Equals");
            Assert.AreEqual(expectedSyntax, Equality.IsNullableEquals(expression, default, default, out var left, out var right));
            if (expectedSyntax)
            {
                Assert.AreEqual("x", left.ToString());
                Assert.AreEqual("y", right.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expectedSymbol, Equality.IsNullableEquals(expression, semanticModel, CancellationToken.None, out left, out right));
            if (expectedSymbol)
            {
                Assert.AreEqual("x", left.ToString());
                Assert.AreEqual("y", right.ToString());
            }
        }

        [TestCase("M(string x, string y) => string.Equals(x, y, StringComparison.Ordinal)",               true,  true)]
        [TestCase("M(string x, string y) => String.Equals(x, y, StringComparison.Ordinal)",               true,  true)]
        [TestCase("M(string x, string y) => System.String.Equals(x, y, System.StringComparison.Ordinal)", true,  true)]
        [TestCase("M(string x, string y) => string.Equals(x, y)",                                         false, false)]
        [TestCase("M(string x, string y) => object.Equals(x, y)",                                         false, false)]
        [TestCase("M(string x, string y) => Object.Equals(x, y)",                                         false, false)]
        public void IsStringEquals(string check, bool expectedSyntax, bool expectedSymbol)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

    class C
    {
        bool M(string x, string y) => string.Equals(x, y,StringComparison.Ordinal);
    }
}".AssertReplace("M(string x, string y) => string.Equals(x, y,StringComparison.Ordinal)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation("Equals");
            Assert.AreEqual(expectedSyntax, Equality.IsStringEquals(expression, default, default, out var left, out var right, out var stringComparison));
            if (expectedSyntax)
            {
                Assert.AreEqual("x", left.ToString());
                Assert.AreEqual("y", right.ToString());
                StringAssert.Contains("StringComparison.Ordinal", stringComparison.ToString());
            }

            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expectedSymbol, Equality.IsStringEquals(expression, semanticModel, CancellationToken.None, out left, out right, out stringComparison));
            if (expectedSymbol)
            {
                Assert.AreEqual("x", left.ToString());
                Assert.AreEqual("y", right.ToString());
                StringAssert.Contains("StringComparison.Ordinal", stringComparison.ToString());
            }
        }

        [TestCase("M(string x, string y) => StringComparer.Ordinal.Equals(x, y)",            "StringComparer.Ordinal",        "x",  "y",  true)]
        [TestCase("M(string x, string y) => System.StringComparer.Ordinal.Equals(x, y)",     "System.StringComparer.Ordinal", "x",  "y",  true)]
        [TestCase("M(StringComparer comparer, string x, string y) => comparer.Equals(x, y)", "comparer",                      "x",  "y",  true)]
        [TestCase("M(string x, string y) => Object.Equals(x, y)",                            null,                            null, null, false)]
        [TestCase("M(string x, int y) => StringComparer.Ordinal.Equals((x, y)",              null,                            null, null, false)]
        public void IsEqualityComparerEquals(string check, string expectedComparer, string expectedLeft, string expectedRight, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Runtime.CompilerServices;

    class C
    {
        bool M(string x, string y) => StringComparer.Ordinal.Equals(x, y);
    }
}".AssertReplace("M(string x, string y) => StringComparer.Ordinal.Equals(x, y)", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.FindInvocation("Equals");
            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(expected, Equality.IsEqualityComparerEquals(expression, semanticModel, CancellationToken.None, out var comparer, out var left, out var right));
            if (expected)
            {
                Assert.AreEqual(expectedComparer, comparer.ToString());
                Assert.AreEqual(expectedLeft,     left.ToString());
                Assert.AreEqual(expectedRight,    right.ToString());
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

        [Test]
        public void IsOverridenWhenTrue()
        {
            var code = @"
namespace N
{
    class C
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType();
        }

        public override int GetHashCode() => 1;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var declaration = syntaxTree.FindClassDeclaration("C");
            Assert.AreEqual(true, Equality.IsOverriden(declaration));
            var compilation = CSharpCompilation.Create("test", new[] {syntaxTree}, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(declaration, CancellationToken.None, out var type));
            Assert.AreEqual(true, Equality.IsOverriden(type));
        }
    }
}

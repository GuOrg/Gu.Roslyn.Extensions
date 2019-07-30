namespace Gu.Roslyn.AnalyzerExtensions.Tests.MemberPathTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class Equals
    {
        [Test]
        public static void SimpleValue()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        private int value;

        public int Value
        {
            get => this.value;
            set => this.value = value;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var getExpression = syntaxTree.FindAccessorDeclaration("get => this.value;").ExpressionBody.Expression;
            var setExpression = ((AssignmentExpressionSyntax)syntaxTree.FindAccessorDeclaration("set => this.value = value;").ExpressionBody.Expression).Left;
            Assert.AreEqual(true, MemberPath.Equals(getExpression, setExpression));
        }

        [TestCase("get => this.value1;", "set => this.value1 = value;", true)]
        [TestCase("get => this.value1;", "set => value1 = value;", true)]
        [TestCase("get => value1;", "set => this.value1 = value;", true)]
        [TestCase("get => value1;", "set => value1 = value;", true)]
        [TestCase("get => this.value1;", "set => this.value2 = value;", false)]
        [TestCase("get => this.value1;", "set => value2 = value;", false)]
        [TestCase("get => value1;", "set => this.value2 = value;", false)]
        [TestCase("get => value1;", "set => value2 = value;", false)]
        public static void Simple(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;

        public int Value
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}";
            testCode = testCode.AssertReplace("get => this.value1;", getter)
                               .AssertReplace("set => this.value1 = value;", setter);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var getExpression = syntaxTree.FindAccessorDeclaration(getter).ExpressionBody.Expression;
            var setExpression = ((AssignmentExpressionSyntax)syntaxTree.FindAccessorDeclaration(setter).ExpressionBody.Expression).Left;
            Assert.AreEqual(expected, MemberPath.Equals(getExpression, setExpression));
        }

        [TestCase("get => this.f1.Value1;", "set => this.f1.Value1 = value;", true)]
        [TestCase("get => this.f1.Value1;", "set => f1.Value1 = value;", true)]
        [TestCase("get => f1.Value1;", "set => this.f1.Value1 = value;", true)]
        [TestCase("get => f1.Value1;", "set => f1.Value1 = value;", true)]
        [TestCase("get => this.f1?.Value1;", "set => this.f1.Value1 = value;", true)]
        [TestCase("get => this.f1.Value1;", "set => f1?.Value1 = value;", true)]
        [TestCase("get => f1?.Value1;", "set => this.f1?.Value1 = value;", true)]
        [TestCase("get => f1?.P?.Value1;", "set => this.f1.P.Value1 = value;", true)]
        [TestCase("get => this.f1.Value1;", "set => this.f2.Value1 = value;", false)]
        [TestCase("get => this.f1.Value1;", "set => f2.Value1 = value;", false)]
        [TestCase("get => f1.Value1;", "set => this.f2.Value1 = value;", false)]
        [TestCase("get => f1.Value1;", "set => f2.Value1 = value;", false)]
        [TestCase("get => this.f1.Value1;", "set => this.f1 = value;", false)]
        [TestCase("get => this.f1.Value1;", "set => this.f2 = value;", false)]
        [TestCase("get => this.f1.Value1;", "set => f1 = value;", false)]
        [TestCase("get => this.f1.Value1;", "set => f2 = value;", false)]
        [TestCase("get => f1.Value1;", "set => this.f1 = value;", false)]
        [TestCase("get => f1.Value1;", "set => this.f2 = value;", false)]
        public static void Nested(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace N
{
    public class C1
    {
        public int Value1;
        public int Value2;
        public C1 P;
    }

    public class C2
    {
        private C1 f1 = new C1();
        private C1 f2 = new C1();

        public int Value
        {
            get => this.f1.Value1;
            set => this.f2.Value1 = value;
        }
    }
}";
            testCode = testCode.AssertReplace("get => this.f1.Value1;", getter)
                               .AssertReplace("set => this.f2.Value1 = value;", setter);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var getExpression = syntaxTree.FindAccessorDeclaration(getter).ExpressionBody.Expression;
            var setExpression = ((AssignmentExpressionSyntax)syntaxTree.FindAccessorDeclaration(setter).ExpressionBody.Expression).Left;
            Assert.AreEqual(expected, MemberPath.Equals(getExpression, setExpression));
        }
    }
}

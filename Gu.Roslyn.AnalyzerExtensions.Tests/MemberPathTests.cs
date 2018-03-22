namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    internal class MemberPathTests
    {
        [TestCase("get => this.value;")]
        [TestCase("set => this.value = value;")]
        public void FindBestMatchAccessorDeclarationSyntax(string accessor)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
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
            Assert.AreEqual(accessor, syntaxTree.FindAccessorDeclaration(accessor).ToString());
        }

        [TestCase("get => this.value1;", "set => this.value1 = value;", true)]
        [TestCase("get => this.value1;", "set => value1 = value;", true)]
        [TestCase("get => value1;", "set => this.value1 = value;", true)]
        [TestCase("get => value1;", "set => value1 = value;", true)]
        [TestCase("get => this.value1;", "set => this.value2 = value;", false)]
        [TestCase("get => this.value1;", "set => value2 = value;", false)]
        [TestCase("get => value1;", "set => this.value2 = value;", false)]
        [TestCase("get => value1;", "set => value2 = value;", false)]
        public void EqualsSimple(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
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

        [TestCase("get => this.bar1.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => bar1.Value1 = value;", true)]
        [TestCase("get => bar1.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => bar1.Value1;", "set => bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1?.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => bar1?.Value1 = value;", true)]
        [TestCase("get => bar1?.Value1;", "set => this.bar1?.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar2.Value1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar2.Value1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar2.Value1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => bar2.Value1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar2 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar2 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar2 = value;", false)]
        public void EqualsNested(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Bar
    {
        public int Value1;
        public int Value2;
    }

    public class Foo
    {
        private Bar bar1 = new Bar();
        private Bar bar2 = new Bar();

        public int Value
        {
            get => this.bar1.Value1;
            set => this.bar2.Value1 = value;
        }
    }
}";
            testCode = testCode.AssertReplace("get => this.bar1.Value1;", getter)
                               .AssertReplace("set => this.bar2.Value1 = value;", setter);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var getExpression = syntaxTree.FindAccessorDeclaration(getter).ExpressionBody.Expression;
            var setExpression = ((AssignmentExpressionSyntax)syntaxTree.FindAccessorDeclaration(setter).ExpressionBody.Expression).Left;
            Assert.AreEqual(expected, MemberPath.Equals(getExpression, setExpression));
        }

        [TestCase("get => this.value1;", "set => this.value1 = value;", true)]
        [TestCase("get => this.value1;", "set => value1 = value;", true)]
        [TestCase("get => value1;", "set => this.value1 = value;", true)]
        [TestCase("get => value1;", "set => value1 = value;", true)]
        [TestCase("get => this.value1;", "set => this.value2 = value;", false)]
        [TestCase("get => this.value1;", "set => value2 = value;", false)]
        [TestCase("get => value1;", "set => this.value2 = value;", false)]
        [TestCase("get => value1;", "set => value2 = value;", false)]
        public void IntersectsSimple(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
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
            Assert.AreEqual(expected, MemberPath.Intersects(getExpression, setExpression));
        }

        [TestCase("get => this.bar1.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => bar1.Value1 = value;", true)]
        [TestCase("get => bar1.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => bar1.Value1;", "set => bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1?.Value1;", "set => this.bar1.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => bar1?.Value1 = value;", true)]
        [TestCase("get => bar1?.Value1;", "set => this.bar1?.Value1 = value;", true)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar2.Value1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar2.Value1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar2.Value1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => bar2.Value1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => this.bar2 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar1 = value;", false)]
        [TestCase("get => this.bar1.Value1;", "set => bar2 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar1 = value;", false)]
        [TestCase("get => bar1.Value1;", "set => this.bar2 = value;", false)]
        public void IntersectsNested(string getter, string setter, bool expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Bar
    {
        public int Value1;
        public int Value2;
    }

    public class Foo
    {
        private Bar bar1 = new Bar();
        private Bar bar2 = new Bar();

        public int Value
        {
            get => this.bar1.Value1;
            set => this.bar2.Value1 = value;
        }
    }
}";
            testCode = testCode.AssertReplace("get => this.bar1.Value1;", getter)
                               .AssertReplace("set => this.bar2.Value1 = value;", setter);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var getExpression = syntaxTree.FindAccessorDeclaration(getter).ExpressionBody.Expression;
            var setExpression = ((AssignmentExpressionSyntax)syntaxTree.FindAccessorDeclaration(setter).ExpressionBody.Expression).Left;
            Assert.AreEqual(expected, MemberPath.Intersects(getExpression, setExpression));
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class PropertyDeclarationSyntaxExtTests
    {
        [TestCase("Value1", "get { return this.value1; }")]
        [TestCase("Value2", "private get { return value2; }")]
        public static void TryGetGetter(string propertyName, string getter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;

        public int Value1
        {
            get { return this.value1; }
            set { this.value1 = value; }
        }

        public int Value2
        {
            private get { return value2; }
            set { value2 = value; }
        }
    }
}");
            var property = syntaxTree.FindPropertyDeclaration(propertyName);
            Assert.AreEqual(true, property.TryGetGetter(out var result));
            Assert.AreEqual(getter, result.ToString());
        }

        [TestCase("Value1", "get { return this.value1; }")]
        [TestCase("Value2", "private get { return value2; }")]
        public static void Getter(string propertyName, string getter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;

        public int Value1
        {
            get { return this.value1; }
            set { this.value1 = value; }
        }

        public int Value2
        {
            private get { return value2; }
            set { value2 = value; }
        }
    }
}");
            var property = syntaxTree.FindPropertyDeclaration(propertyName);
            Assert.AreEqual(getter, property.Getter().ToString());
        }

        [TestCase("Value1", "set { this.value1 = value; }")]
        [TestCase("Value2", "private set { value2 = value; }")]
        public static void TryGetSetter(string propertyName, string setter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;

        public int Value1
        {
            get { return this.value1; }
            set { this.value1 = value; }
        }

        public int Value2
        {
            get { return value2; }
            private set { value2 = value; }
        }
    }
}");
            var property = syntaxTree.FindPropertyDeclaration(propertyName);
            Assert.AreEqual(true, property.TryGetSetter(out var result));
            Assert.AreEqual(setter, result.ToString());
        }

        [TestCase("Value1", "set { this.value1 = value; }")]
        [TestCase("Value2", "private set { value2 = value; }")]
        public static void Setter(string propertyName, string setter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;

        public int Value1
        {
            get { return this.value1; }
            set { this.value1 = value; }
        }

        public int Value2
        {
            get { return value2; }
            private set { value2 = value; }
        }
    }
}");
            var property = syntaxTree.FindPropertyDeclaration(propertyName);
            Assert.AreEqual(setter, property.Setter().ToString());
        }

        [TestCase("GetOnly", true)]
        [TestCase("ExpressionBody", false)]
        [TestCase("ExpressionBodyGetter", false)]
        [TestCase("StatementBodyGetter", false)]
        [TestCase("AutoGetSet", true)]
        [TestCase("ExpressionBodyBackingField", false)]
        [TestCase("StatementBodyBackingField", false)]
        public static void IsAutoProperty(string name, bool expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int GetOnly { get; }

        public int ExpressionBody => this.GetOnly;

        public int ExpressionBodyGetter
        {
            get => this.GetOnly;
        }

        public int StatementBodyGetter
        {
            get { return this.GetOnly; }
        }

        public int AutoGetSet { get; set; }

        public int ExpressionBodyBackingField
        {
            get => this.AutoGetSet;
            set => this.AutoGetSet = value;
        }

        public int StatementBodyBackingField
        {
            get { return this.AutoGetSet; }
            set { this.AutoGetSet = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var property = syntaxTree.FindPropertyDeclaration(name);
            Assert.AreEqual(expected, property.IsAutoProperty());
        }

        [TestCase("GetOnly", true)]
        [TestCase("ExpressionBody", false)]
        [TestCase("ExpressionBodyGetter", false)]
        [TestCase("StatementBodyGetter", false)]
        [TestCase("AutoGetSet", false)]
        [TestCase("ExpressionBodyBackingField", false)]
        [TestCase("StatementBodyBackingField", false)]
        public static void IsGetOnly(string name, bool expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int GetOnly { get; }

        public int ExpressionBody => this.GetOnly;

        public int ExpressionBodyGetter
        {
            get => this.GetOnly;
        }

        public int StatementBodyGetter
        {
            get { return this.GetOnly; }
        }

        public int AutoGetSet { get; set; }

        public int ExpressionBodyBackingField
        {
            get => this.AutoGetSet;
            set => this.AutoGetSet = value;
        }

        public int StatementBodyBackingField
        {
            get { return this.AutoGetSet; }
            set { this.AutoGetSet = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var property = syntaxTree.FindPropertyDeclaration(name);
            Assert.AreEqual(expected, property.IsGetOnly());
        }

        [TestCase("ExpressionBody")]
        [TestCase("ExpressionBodyGetter")]
        [TestCase("StatementBodyGetter")]
        [TestCase("ExpressionBodyBackingField")]
        [TestCase("StatementBodyBackingField")]
        public static void TryGetBackingFieldWhenTrue(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        private int backingField;

        public int ExpressionBody => this.backingField;

        public int ExpressionBodyGetter
        {
            get => this.backingField;
        }

        public int StatementBodyGetter
        {
            get { return this.backingField; }
        }

        public int ExpressionBodyBackingField
        {
            get => this.backingField;
            set => this.backingField = value;
        }

        public int StatementBodyBackingField
        {
            get { return this.backingField; }
            set { this.backingField = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var property = syntaxTree.FindPropertyDeclaration(name);
            Assert.AreEqual(true, property.TryGetBackingField(out var backingField));
            Assert.AreEqual("private int backingField;", backingField.ToString());
        }

        [TestCase("GetOnly")]
        [TestCase("ExpressionBody")]
        [TestCase("ExpressionBodyGetter")]
        [TestCase("StatementBodyGetter")]
        [TestCase("AutoGetSet")]
        [TestCase("ExpressionBodyBackingField")]
        [TestCase("StatementBodyBackingField")]
        public static void TryGetBackingFieldWhenFalse(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int GetOnly { get; }

        public int ExpressionBody => this.GetOnly;

        public int ExpressionBodyGetter
        {
            get => this.GetOnly;
        }

        public int StatementBodyGetter
        {
            get { return this.GetOnly; }
        }

        public int AutoGetSet { get; set; }

        public int ExpressionBodyBackingField
        {
            get => this.AutoGetSet;
            set => this.AutoGetSet = value;
        }

        public int StatementBodyBackingField
        {
            get { return this.AutoGetSet; }
            set { this.AutoGetSet = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var property = syntaxTree.FindPropertyDeclaration(name);
            Assert.AreEqual(false, property.TryGetBackingField(out _));
        }
    }
}

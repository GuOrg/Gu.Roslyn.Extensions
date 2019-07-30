namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class AddBackingField
    {
        [Test]
        public static async Task WhenNoBackingFields()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        public int Meh1 = 1;
        private int meh2;

        public int Value { get; set; }
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var property = editor.OriginalRoot.SyntaxTree.FindPropertyDeclaration("Value");
            var field = editor.AddBackingField(property);
            Assert.AreEqual("privateint value;", field.ToFullString());
            var expected = @"
namespace N
{
    public class C
    {
        public int Meh1 = 1;
        private int meh2;
        private int value;

        public int Value { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task WhenNameCollision()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        private int value;

        public int Value { get; set; }
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var property = editor.OriginalRoot.SyntaxTree.FindPropertyDeclaration("Value");
            var field = editor.AddBackingField(property);
            Assert.AreEqual("privateint value_;", field.ToFullString());
            var expected = @"
namespace N
{
    public class C
    {
        private int value;
        private int value_;

        public int Value { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task Between()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        private int value1;
        private int value3;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        public int Value2 { get; set; }

        public int Value3
        {
            get => this.value3;
            set => this.value3 = value;
        }
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var property = editor.OriginalRoot.SyntaxTree.FindPropertyDeclaration("Value2");
            var field = editor.AddBackingField(property);
            Assert.AreEqual("privateint value2;", field.ToFullString());
            var expected = @"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;
        private int value3;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        public int Value2 { get; set; }

        public int Value3
        {
            get => this.value3;
            set => this.value3 = value;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AdjacentToProperty()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int value1;
        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        public int Value2 { get; set; }
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var property = editor.OriginalRoot.SyntaxTree.FindPropertyDeclaration("Value2");
            var field = editor.AddBackingField(property);
            Assert.AreEqual("privateint value2;", field.ToFullString());
            var expected = @"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int value1;
        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        private int value2;
        public int Value2 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AdjacentToPropertyNewLine()
        {
            var testCode = @"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int value1;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        public int Value2 { get; set; }
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var property = editor.OriginalRoot.SyntaxTree.FindPropertyDeclaration("Value2");
            var field = editor.AddBackingField(property);
            Assert.AreEqual("privateint value2;", field.ToFullString());
            var expected = @"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int value1;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }

        private int value2;

        public int Value2 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}

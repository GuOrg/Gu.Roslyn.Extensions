namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public partial class DocumentEditorExtTests
    {
        public class AddField
        {
            [Test]
            public async Task AddPrivateField()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = SyntaxNodeExt.FindClassDeclaration(editor.OriginalRoot.SyntaxTree, "Foo");

                var expected = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;
        private bool disposable;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }
    }
}";

                var newField = (FieldDeclarationSyntax)editor.Generator.FieldDeclaration(
                    "disposable",
                    SyntaxFactory.ParseTypeName("bool"),
                    Accessibility.Private);
                editor.AddField(containingType, newField);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task AddBackingFieldWhenNoBackingFields()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
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
namespace RoslynSandbox
{
    public class Foo
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
            public async Task AddBackingFieldWhenNameCollision()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
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
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;
        private int value_;

        public int Value { get; set; }
    }
}";
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task AddBackingFieldAdjacentToProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
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
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
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
}

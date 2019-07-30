namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class AddField
    {
        [TestCase("public static readonly int NewField = 1;")]
        [TestCase("private int newField;")]
        public static async Task AddPrivateFieldWhenEmpty(string declaration)
        {
            var code = @"
namespace N
{
    class C
    {
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    class C
    {
        private int newField;
    }
}".AssertReplace("private int newField;", declaration);

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(declaration);
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddPublicFieldWhenPrivateExists()
        {
            var code = @"
namespace N
{
    class C
    {
        private int f;
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    class C
    {
        public int NewField;

        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewField;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddPublicFieldWhenPublicAndPrivateExists()
        {
            var code = @"
namespace N
{
    class C
    {
        public int F;

        private int f;
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    class C
    {
        public int F;
        public int NewField;

        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewField;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddPrivateFieldWhenPublicExists()
        {
            var code = @"
namespace N
{
    class C
    {
        public int F;
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    class C
    {
        public int F;

        private int newField;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private int newField;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddPrivateFieldWhenPrivateExists()
        {
            var code = @"
namespace N
{
    class C
    {
        private int f;
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    class C
    {
        private int f;
        private int newField;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private int newField;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task TypicalClass()
        {
            var code = @"
namespace N
{
    public abstract class C
    {
        public int Field1 = 1;
        private int field1;

        public C()
        {
        }

        private C(int i)
        {
        }

        public int P1 { get; set; }

        public void M1()
        {
        }

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
        {
        }
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    public abstract class C
    {
        public int Field1 = 1;
        private int field1;
        private bool disposable;

        public C()
        {
        }

        private C(int i)
        {
        }

        public int P1 { get; set; }

        public void M1()
        {
        }

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
        {
        }
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private bool disposable;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}

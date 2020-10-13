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
        public static async Task WhenEmptyClass(string declaration)
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
        public static async Task AddDependentFields()
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
        private const int F1 = 1;
        private const int F2 = F1;
    }
}";

            _ = editor.AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"private const int F1 = 1;"))
                      .AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"private const int F2 = F1;"));
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddFieldWithSingleLineDocsAfterFieldInConditional()
        {
            var code = @"
namespace N
{
    public class C
    {
#if true
        private const int F1 = 1;
#endif
    }
}";
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            var expected = @"
namespace N
{
    public class C
    {
#if true
        private const int F1 = 1;
#endif

        /// <summary> Text </summary>
        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary> Text </summary>
private int f;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddFieldWithSingleLineDocs()
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

        /// <summary> Text </summary>
        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary> Text </summary>
private int f;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddFieldWithMultiLineDocs()
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

        /// <summary>
        /// Text
        /// </summary>
        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary>
/// Text
/// </summary>
private int f;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddIndentedFieldWithSingleLineDocs()
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

        /// <summary> Text </summary>
        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"
        /// <summary> Text </summary>
        private int f;");
            _ = editor.AddField(containingType, newField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddIndentedFieldWithMultiLineDocs()
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

        /// <summary>
        /// Text
        /// </summary>
        private int f;
    }
}";

            var newField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"
        /// <summary>
        /// Text
        /// </summary>
        private int f;");
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
        public static async Task AddPublicWhenPublicAndPrivateExists()
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
        public static async Task AddPrivateWhenPublicExists()
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
        public static async Task AddPrivateWhenPrivateExists()
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
        public static async Task AddPrivateAndPublicWhenPublicExists()
        {
            var code = @"
namespace N
{
    class C
    {
        /// <summary> F1 </summary>
        public static readonly int F1;
    }
}";
            var sln = CodeFactory.CreateSolution(code);

            var expected = @"
namespace N
{
    class C
    {
        /// <summary> F1 </summary>
        public static readonly int F1;

        private static readonly int f2;

        /// <summary> F2 </summary>
        public static readonly int F2 = f2;
    }
}";
            var privateField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private static readonly int f2;");
            var publicField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary> F2 </summary>
public static readonly int F2 = f2;");

            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            _ = editor.AddField(containingType, privateField)
                      .AddField(containingType, publicField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());

            editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            _ = editor.AddField(containingType, publicField)
                      .AddField(containingType, privateField);
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());

            editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            editor.ReplaceNode(containingType, containingType.AddField(privateField).AddField(publicField));
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());

            editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");

            editor.ReplaceNode(containingType, containingType.AddField(publicField).AddField(privateField));
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddPublicAndPrivateWhenPublicExists()
        {
            var code = @"
namespace N
{
    class C
    {
        /// <summary> F1 </summary>
        public static readonly int F1;
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
        /// <summary> F1 </summary>
        public static readonly int F1;

        private static readonly int f2;

        /// <summary> F2 </summary>
        public static readonly int F2 = f2;
    }
}";

            _ = editor.AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary> F2 </summary>
public static readonly int F2 = f2;"))
                      .AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private static readonly int f2;"));
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddInternalAndPrivateWhenPublicAndPrivateExists()
        {
            var code = @"
namespace N
{
    class C
    {
        /// <summary> F1 </summary>
        public static readonly int F1;

        /// <summary> f3 </summary>
        private static readonly int f3;
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
        /// <summary> F1 </summary>
        public static readonly int F1;

        private static readonly int f2;

        /// <summary> F2 </summary>
        internal static readonly int F2 = f2;

        /// <summary> f3 </summary>
        private static readonly int f3;
    }
}";

            _ = editor.AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(@"/// <summary> F2 </summary>
internal static readonly int F2 = f2;"))
                      .AddField(containingType, (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("private static readonly int f2;"));
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

namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class Move
    {
        [Test]
        public static void MoveFieldBeforeFirst()
        {
            var code = @"
namespace N
{
    class C
    {
        public static readonly int F1 = 1;

        private static readonly int F2 = 2;
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<FieldDeclarationSyntax>("F2"), editor.OriginalRoot.Find<FieldDeclarationSyntax>("F1"));

            var expected = @"
namespace N
{
    class C
    {
        private static readonly int F2 = 2;

        public static readonly int F1 = 1;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveFieldBeforeFirstWithComments()
        {
            var code = @"
namespace N
{
    class C
    {
        // F1
        public static readonly int F1 = 1;

        // F2
        private static readonly int F2 = 2;
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<FieldDeclarationSyntax>("F2"), editor.OriginalRoot.Find<FieldDeclarationSyntax>("F1"));

            var expected = @"
namespace N
{
    class C
    {
        // F2
        private static readonly int F2 = 2;

        // F1
        public static readonly int F1 = 1;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveFieldFirstAfterLast()
        {
            var code = @"
namespace N
{
    class C
    {
        public static readonly int F1 = 1;

        private static readonly int F2 = 2;
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<FieldDeclarationSyntax>("F1"), editor.OriginalRoot.Find<FieldDeclarationSyntax>("F2"));

            var expected = @"
namespace N
{
    class C
    {
        private static readonly int F2 = 2;

        public static readonly int F1 = 1;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveFieldFirstAfterLastWithComments()
        {
            var code = @"
namespace N
{
    class C
    {
        // F1
        public static readonly int F1 = 1;

        // F2
        private static readonly int F2 = 2;
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<FieldDeclarationSyntax>("F1"), editor.OriginalRoot.Find<FieldDeclarationSyntax>("F2"));

            var expected = @"
namespace N
{
    class C
    {
        // F2
        private static readonly int F2 = 2;

        // F1
        public static readonly int F1 = 1;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MovePropertyBeforeFirst()
        {
            var code = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P2 { get; set; }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P1"));

            var expected = @"
namespace N
{
    class C
    {
        public int P2 { get; set; }

        public int P1 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MovePropertyBeforeSecond()
        {
            var code = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P2 { get; set; }

        public int P3 { get; set; }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P3"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"));

            var expected = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P3 { get; set; }

        public int P2 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementBeforeFirst()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var b = 1;
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<StatementSyntax>("var b = 1;"), editor.OriginalRoot.Find<StatementSyntax>("var a = 1;"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            var b = 1;
            var a = 1;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementBeforeIf()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            if (true)
            {
            }

            var b = 1;
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<StatementSyntax>("var b = 1;"), editor.OriginalRoot.Find<StatementSyntax>("if (true)"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            var b = 1;
            if (true)
            {
            }
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementBeforeSecond()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var b = 1;
            var c = 1;
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveBefore(editor.OriginalRoot.Find<StatementSyntax>("var c = 1;"), editor.OriginalRoot.Find<StatementSyntax>("var b = 1;"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var c = 1;
            var b = 1;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MovePropertyAfterFirst()
        {
            var code = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P2 { get; set; }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P1"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"));

            var expected = @"
namespace N
{
    class C
    {
        public int P2 { get; set; }

        public int P1 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MovePropertyAfterSecond()
        {
            var code = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P2 { get; set; }

        public int P3 { get; set; }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P3"));

            var expected = @"
namespace N
{
    class C
    {
        public int P1 { get; set; }

        public int P3 { get; set; }

        public int P2 { get; set; }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementAfterFirst()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var b = 1;
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<StatementSyntax>("var a = 1;"), editor.OriginalRoot.Find<StatementSyntax>("var b = 1;"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            var b = 1;
            var a = 1;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementAfterIf()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            if (true)
            {
            }
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<StatementSyntax>("var a = 1;"), editor.OriginalRoot.Find<StatementSyntax>("if (true)"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            if (true)
            {
            }

            var a = 1;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static void MoveStatementAfterSecond()
        {
            var code = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var b = 1;
            var c = 1;
        }
    }
}";
            var editor = CreateDocumentEditor(code);
            _ = editor.MoveAfter(editor.OriginalRoot.Find<StatementSyntax>("var b = 1;"), editor.OriginalRoot.Find<StatementSyntax>("var c = 1;"));

            var expected = @"
namespace N
{
    class C
    {
        C()
        {
            var a = 1;
            var c = 1;
            var b = 1;
        }
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        private static DocumentEditor CreateDocumentEditor(string code)
        {
            var sln = CodeFactory.CreateSolution(code);
            return DocumentEditor.CreateAsync(sln.Projects.Single().Documents.Single()).Result;
        }
    }
}

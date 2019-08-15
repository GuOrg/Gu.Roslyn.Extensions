namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class Move
    {
        [Test]
        public static async Task MovePropertyBeforeFirst()
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
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
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
        public static async Task MovePropertyBeforeSecond()
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
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
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
        public static async Task MoveStatementBeforeFirst()
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
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
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
        public static async Task MoveStatementBeforeSecond()
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
            var sln = CodeFactory.CreateSolution(code);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
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
    }
}

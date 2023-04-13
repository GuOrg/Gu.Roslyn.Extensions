namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests;

using System.Linq;
using System.Threading.Tasks;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;

public static class AddEvent
{
    [Test]
    public static async Task AddEventFieldDeclarationSyntax()
    {
        var code = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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
    }
}";
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
        var eventDeclaration = (EventFieldDeclarationSyntax)editor.Generator.EventDeclaration("SomeEvent", SyntaxFactory.ParseTypeName("System.EventHandler"), Accessibility.Public);
        var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
        _ = editor.AddEvent(containingType, eventDeclaration);
        var expected = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

        public C()
        {
        }

        private C(int i)
        {
        }

        public event System.EventHandler SomeEvent;

        public int P1 { get; set; }

        public void M1()
        {
        }
    }
}";
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task AddEventDeclarationSyntax()
    {
        var code = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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
    }
}";
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
        var eventDeclaration = (EventDeclarationSyntax)editor.Generator.CustomEventDeclaration("SomeEvent", SyntaxFactory.ParseTypeName("System.EventHandler"), Accessibility.Public);
        var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
        _ = editor.AddEvent(containingType, eventDeclaration);
        var expected = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

        public C()
        {
        }

        private C(int i)
        {
        }

        public event System.EventHandler SomeEvent
        {
            add
            {
            }

            remove
            {
            }
        }

        public int P1 { get; set; }

        public void M1()
        {
        }
    }
}";
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }
}

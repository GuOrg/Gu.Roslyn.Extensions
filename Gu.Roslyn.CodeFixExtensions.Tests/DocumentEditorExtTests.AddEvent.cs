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
        public class AddEvent
        {
            [Test]
            public async Task AddEventFieldDeclarationSyntax()
            {
                var testCode = @"
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

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
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

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }
    }
}";
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task AddEventDeclarationSyntax()
            {
                var testCode = @"
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

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
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

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }
    }
}";
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}

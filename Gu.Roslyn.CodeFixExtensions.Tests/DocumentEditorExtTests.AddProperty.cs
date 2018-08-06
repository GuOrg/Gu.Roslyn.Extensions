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
        public class AddProperty
        {
            [Test]
            public async Task AddPropertyDeclarationSyntax()
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
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var declaration = (PropertyDeclarationSyntax)editor.Generator.PropertyDeclaration("Property", SyntaxFactory.ParseTypeName("int"), Accessibility.Public);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("Foo");
                editor.AddProperty(containingType, declaration);
                var expected = @"
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

        public int Property
        {
            get
            {
            }

            set
            {
            }
        }

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

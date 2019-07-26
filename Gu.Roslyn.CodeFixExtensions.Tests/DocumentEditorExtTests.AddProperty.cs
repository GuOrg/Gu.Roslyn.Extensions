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
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var declaration = (PropertyDeclarationSyntax)editor.Generator.PropertyDeclaration("Property", SyntaxFactory.ParseTypeName("int"), Accessibility.Public);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                _ = editor.AddProperty(containingType, declaration);
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

        public int P1 { get; set; }

        public int Property
        {
            get
            {
            }

            set
            {
            }
        }

        public void M1()
        {
        }
    }
}";
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}

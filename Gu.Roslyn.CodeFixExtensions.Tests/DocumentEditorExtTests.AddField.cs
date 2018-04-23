namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public partial class DocumentEditorExtTests
    {
        public class AddField
        {
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

        }
    }
}

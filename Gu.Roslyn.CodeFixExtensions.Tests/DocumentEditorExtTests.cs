namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public partial class DocumentEditorExtTests
    {
        [Test]
        public async Task ReplaceToken()
        {
            var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("Foo");
            editor.ReplaceToken(containingType.Modifiers.First(), SyntaxFactory.Token(SyntaxKind.InternalKeyword));
            var expected = @"
namespace RoslynSandbox
{
    internal class Foo
    {
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}

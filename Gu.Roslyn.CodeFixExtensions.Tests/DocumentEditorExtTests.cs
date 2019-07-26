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
namespace N
{
    public class C
    {
    }
}";
            var sln = CodeFactory.CreateSolution(testCode);
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
            _ = editor.ReplaceToken(containingType.Modifiers.First(), SyntaxFactory.Token(SyntaxKind.InternalKeyword));
            var expected = @"
namespace N
{
    internal class C
    {
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}

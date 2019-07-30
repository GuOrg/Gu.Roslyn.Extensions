namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class ReplaceToken
    {
        [Test]
        public static async Task Modifier()
        {
            var code = @"
namespace N
{
    public class C
    {
    }
}";
            var sln = CodeFactory.CreateSolution(code);
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

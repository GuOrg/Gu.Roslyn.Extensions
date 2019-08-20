namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyEventAccess
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static async Task WhenUnknown()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        C(int i, double d)
        {
        }

        void M()
        {
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyEventAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("E?.Invoke();",      CodeStyleResult.No)]
        [TestCase("E.Invoke();",       CodeStyleResult.No)]
        [TestCase("E();",              CodeStyleResult.No)]
        [TestCase("this.E?.Invoke();", CodeStyleResult.Yes)]
        [TestCase("this.E.Invoke();",  CodeStyleResult.Yes)]
        [TestCase("this.E();",         CodeStyleResult.Yes)]
        public static async Task RaisingInMethod(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    using System;

    class C
    {
        public event Action E;

        void M()
        {
            E?.Invoke();
        }
    }
}".AssertReplace("E?.Invoke();", expression));

            Assert.AreEqual(expected, await editor.QualifyEventAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
        {
            return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
        }
    }
}

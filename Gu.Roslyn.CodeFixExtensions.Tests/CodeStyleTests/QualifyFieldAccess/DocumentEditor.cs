namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyFieldAccess
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
    internal class C
    {
        internal C(int i, double d)
        {
        }

        internal void M()
        {
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("_f1 = 1",      CodeStyleResult.No)]
        [TestCase("this._f1 = 1", CodeStyleResult.Yes)]
        [TestCase("f2 = 1",       CodeStyleResult.No)]
        [TestCase("this.f2 = 1",  CodeStyleResult.Yes)]
        [TestCase("F3 = 1",       CodeStyleResult.No)]
        [TestCase("this.F3 = 1",  CodeStyleResult.Yes)]
        public static async Task AssigningInCtor(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int _f1;
        private int f2;
        public int F3;

        C()
        {
            _f1 = 1;
        }
    }
}".AssertReplace("_f1 = 1", expression));

            Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task IgnoreObjectInitializer()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int p;

        public static C Create(int i) => new C { p = i };
    }
}");

            Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", CodeStyleResult.Yes)]
        [TestCase("f",      CodeStyleResult.No)]
        public static async Task ReturningInMethodExpressionBody(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int f;

        public int M() => this.f;
    }
}".AssertReplace("this.f", expression));

            Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", CodeStyleResult.Yes)]
        [TestCase("f",      CodeStyleResult.No)]
        public static async Task ReturningInPropertyExpressionBody(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int f;

        public int P => this.f;
    }
}".AssertReplace("this.f", expression));

            Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", CodeStyleResult.Yes)]
        [TestCase("f",      CodeStyleResult.No)]
        public static async Task ReturningInPropertyGetterExpressionBody(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int f;

        public int P
        {
            get => this.f;
        }
    }
}".AssertReplace("this.f", expression));

            Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", CodeStyleResult.Yes)]
        [TestCase("f",      CodeStyleResult.No)]
        public static async Task ReturningInPropertyGetterStatementBody(string expression, CodeStyleResult expected)
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private int f;

        public int P
        {
            get { return this.f; }
        }
    }
}".AssertReplace("this.f", expression));

            Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f = 1", CodeStyleResult.Yes)]
        [TestCase("f = 1",      CodeStyleResult.No)]
        [TestCase("",           CodeStyleResult.NotFound)]
        public static async Task FiguresOutFromOtherClass(string expression, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(new[]
            {
                @"
namespace N
{
    class C1
    {
        private int f;

        C1()
        {
            this.f = 1;
        }
    }
}".AssertReplace("this.f = 1", expression),
                @"
namespace N
{
    class C2
    {
    }
}",
            });

            foreach (var document in sln.Projects.Single().Documents)
            {
                var editor = await Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(document).ConfigureAwait(false);
                Assert.AreEqual(expected, await editor.QualifyFieldAccessAsync(CancellationToken.None).ConfigureAwait(false));
            }
        }

        private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
        {
            return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
        }
    }
}

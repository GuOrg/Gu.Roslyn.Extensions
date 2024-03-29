namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyMethodAccess;

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
        var sln = CodeFactory.CreateSolution(@"
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
        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(CodeStyleResult.NotFound, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task CallInCtor(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        C()
        {
            this.M1();
        }

        public void M1() { }

        public static void M2() { }
    }
}".AssertReplace("this.M1()", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M",      CodeStyleResult.No)]
    [TestCase("this.M", CodeStyleResult.Yes)]
    public static async Task UsedInNameof(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public string M() => nameof(this.M);
    }
}".AssertReplace("this.M", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M",      CodeStyleResult.No)]
    [TestCase("this.M", CodeStyleResult.Yes)]
    public static async Task UsedInNameofTwoMethods(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public string M() => nameof(this.M);
        public void M(int i) { }
    }
}".AssertReplace("this.M", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [Test]
    public static async Task UsedInNameofStaticContext()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public void M() { }

        public static string P => nameof(M);
    }
}");
        Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [Test]
    public static async Task UsedInNameofShadowed()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public void M() { }

        public string M2(int M) => nameof(M);
    }
}");
        Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task ExpressionBody(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P => this.M1();

        public int M1() => 1;

        public static int M2() => 2;
    }
}".AssertReplace("this.M1()", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task Assignment(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        C()
        {
            _ = this.M1();
        }

        public int M1() => 1;

        public static int M2() => 2;
    }
}".AssertReplace("this.M1()", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task Argument(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        C()
        {
            _ = Equals(this.M1(), 1);
        }

        public int M1() => 1;

        public static int M2() => 2;
    }
}".AssertReplace("this.M1()", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task CallInSetter(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        private string f;

        public string Name
        {
            get => this.f;
            set
            {
                this.f = value;
                ↓this.M1();
            }
        }

        public void M1() { }

        public static void M2() { }
    }
}".AssertReplace("this.M1()", expression));
        Assert.AreEqual(expected, await editor.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
    {
        return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
    }
}

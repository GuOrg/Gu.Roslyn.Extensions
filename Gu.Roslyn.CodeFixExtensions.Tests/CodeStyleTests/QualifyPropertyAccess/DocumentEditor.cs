namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyPropertyAccess;

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
        Assert.AreEqual(CodeStyleResult.NotFound, await document.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("P = 1",      CodeStyleResult.No)]
    [TestCase("this.P = 1", CodeStyleResult.Yes)]
    public static async Task AssigningInCtor(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        C()
        {
            P = 1;
        }

        public int P { get; }
    }
}".AssertReplace("P = 1", expression));
        Assert.AreEqual(expected, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("P",      CodeStyleResult.No)]
    [TestCase("this.P", CodeStyleResult.Yes)]
    public static async Task UsedInNameof(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P { get; }

        public string M() => nameof(this.P);
    }
}".AssertReplace("this.P", expression));
        Assert.AreEqual(expected, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [Test]
    public static async Task UsedInNameofStaticContext()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P => 1;

        public static string M() => nameof(P);
    }
}");
        Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [Test]
    public static async Task UsedInNameofShadowed()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P => 1;

        public string M(int P) => nameof(P);
    }
}");
        Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [Test]
    public static async Task IgnoreObjectInitializer()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P { get; set; }

        public static C Create(int i) => new C { P = i };
    }
}");
        Assert.AreEqual(CodeStyleResult.NotFound, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("this.P", CodeStyleResult.Yes)]
    [TestCase("P",      CodeStyleResult.No)]
    public static async Task Arrow(string expression, CodeStyleResult expected)
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    class C
    {
        public int P { get; }

        public int M() => this.P;
    }
}".AssertReplace("this.P", expression));
        Assert.AreEqual(expected, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("this.P = 1", CodeStyleResult.Yes)]
    [TestCase("P = 1",      CodeStyleResult.No)]
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
        C1()
        {
            this.P = 1;
        }

        public int P { get; }
    }
}".AssertReplace("this.P = 1", expression),
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
            Assert.AreEqual(expected, await editor.QualifyPropertyAccessAsync(CancellationToken.None).ConfigureAwait(false));
        }
    }

    private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
    {
        return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
    }
}

namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyMethodAccess;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gu.Roslyn.Asserts;
using NUnit.Framework;

public static class Document
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
        var sln = CodeFactory.CreateSolution(@"
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

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task ExpressionBody(string expression, CodeStyleResult expected)
    {
        var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        public int P => this.M1();

        public int M1() => 1;

        public static int M2() => 2;
    }
}".AssertReplace("this.M1()", expression));

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task Assignment(string expression, CodeStyleResult expected)
    {
        var sln = CodeFactory.CreateSolution(@"
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

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task Argument(string expression, CodeStyleResult expected)
    {
        var sln = CodeFactory.CreateSolution(@"
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

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("M1()",      CodeStyleResult.No)]
    [TestCase("this.M1()", CodeStyleResult.Yes)]
    [TestCase("M2()",      CodeStyleResult.NotFound)]
    public static async Task CallInSetter(string expression, CodeStyleResult expected)
    {
        var sln = CodeFactory.CreateSolution(@"
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

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyMethodAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }
}

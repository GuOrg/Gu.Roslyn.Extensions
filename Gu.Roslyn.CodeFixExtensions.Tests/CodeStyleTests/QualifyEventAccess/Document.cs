namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.QualifyEventAccess;

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
        Assert.AreEqual(CodeStyleResult.NotFound, await document.QualifyEventAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }

    [TestCase("E?.Invoke();", CodeStyleResult.No)]
    [TestCase("E.Invoke();", CodeStyleResult.No)]
    [TestCase("E();", CodeStyleResult.No)]
    [TestCase("this.E?.Invoke();", CodeStyleResult.Yes)]
    [TestCase("this.E.Invoke();", CodeStyleResult.Yes)]
    [TestCase("this.E();", CodeStyleResult.Yes)]
    public static async Task RaisingInMethod(string expression, CodeStyleResult expected)
    {
        var sln = CodeFactory.CreateSolution(@"
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

        var document = sln.Projects.Single().Documents.Single();
        Assert.AreEqual(expected, await document.QualifyEventAccessAsync(CancellationToken.None).ConfigureAwait(false));
    }
}

namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UnderscoreFields
{
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
            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(CodeStyleResult.NotFound, await CodeStyle.UnderscoreFieldsAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("private int _f", CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f", CodeStyleResult.No)]
        [TestCase("private readonly int f", CodeStyleResult.No)]
        public static async Task FiguresOutFromDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(
                new[]
                {
                    @"
namespace N
{
    class C1
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration),
                });
            foreach (var document in sln.Projects.Single().Documents)
            {
                Assert.AreEqual(expected, await CodeStyle.UnderscoreFieldsAsync(document, CancellationToken.None).ConfigureAwait(false));
            }
        }

        [TestCase("private int _f", CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f", CodeStyleResult.No)]
        [TestCase("private readonly int f", CodeStyleResult.No)]
        public static async Task FiguresOutFromOtherDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(
                new[]
                {
                    @"
namespace N
{
    class C1
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration),
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
                Assert.AreEqual(expected, await CodeStyle.UnderscoreFieldsAsync(document, CancellationToken.None).ConfigureAwait(false));
            }
        }
    }
}

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
        public static async Task DefaultsToNull()
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
            Assert.AreEqual(null, await CodeStyle.UnderscoreFieldsAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("private int _f", true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f", false)]
        [TestCase("private readonly int f", false)]
        public static async Task FiguresOutFromDocument(string declaration, bool expected)
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

        [TestCase("private int _f", true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f", false)]
        [TestCase("private readonly int f", false)]
        public static async Task FiguresOutFromOtherDocument(string declaration, bool expected)
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

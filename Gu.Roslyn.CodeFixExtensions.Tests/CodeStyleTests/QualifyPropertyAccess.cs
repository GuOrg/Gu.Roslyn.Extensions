namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class QualifyPropertyAccess
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
            Assert.AreEqual(CodeStyleResult.NotFound, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("P = 1",      CodeStyleResult.No)]
        [TestCase("this.P = 1", CodeStyleResult.Yes)]
        public static async Task AssigningInCtor(string expression, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(@"
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

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task IgnoreObjectInitializer()
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        public int P { get; set; }

        public static C Create(int i) => new C { P = i };
    }
}");

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(CodeStyleResult.NotFound, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.P", CodeStyleResult.Yes)]
        [TestCase("P",      CodeStyleResult.No)]
        public static async Task Arrow(string expression, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        public int P { get; }

        public int M() => this.P;
    }
}".AssertReplace("this.P", expression));

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
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
                Assert.AreEqual(expected, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
            }
        }
    }
}

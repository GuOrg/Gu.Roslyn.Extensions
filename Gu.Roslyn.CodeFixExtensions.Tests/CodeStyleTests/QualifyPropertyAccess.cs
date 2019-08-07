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
        public static async Task DefaultsToNull()
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
            Assert.AreEqual(null, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("P = 1", false)]
        [TestCase("this.P = 1", true)]
        public static async Task AssigningInCtor(string expression, bool expected)
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
            Assert.AreEqual(null, await CodeStyle.QualifyPropertyAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.P", true)]
        [TestCase("P",      false)]
        public static async Task Arrow(string expression, bool expected)
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

        [TestCase("this.P = 1", true)]
        [TestCase("P = 1",      false)]
        [TestCase("",           null)]
        public static async Task FiguresOutFromOtherClass(string expression, bool? expected)
        {
            var sln = CodeFactory.CreateSolution(
                new[]
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

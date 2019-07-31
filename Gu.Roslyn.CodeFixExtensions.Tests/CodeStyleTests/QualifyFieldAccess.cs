namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class QualifyFieldAccess
    {
        [Test]
        public static async Task DefaultsToTrue()
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
            Assert.AreEqual(true, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task AssigningUnderscoreInCtor()
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        int _f;
        C()
        {
            _f = 1;
        }
    }
}");

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(false, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task AssigningQualifiedInCtor()
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        int f;
        C()
        {
            this.f = 1;
        }
    }
}");

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(true, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task IgnoreObjectInitializer()
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        private int p;

        public static C Create(int i) => new C { p = i };
    }
}");

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(true, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", true)]
        [TestCase("f", false)]
        public static async Task Arrow(string expression, bool expected)
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        private int f;

        public int M() => this.f;
    }
}".AssertReplace("this.f", expression));

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [Test]
        public static async Task FiguresOutFromOtherClass()
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

        public int M() => _f = 1;
    }
}",
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
                Assert.AreEqual(false, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
            }
        }
    }
}

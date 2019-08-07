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
            Assert.AreEqual(null, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("_f1 = 1", false)]
        [TestCase("this._f1 = 1", true)]
        [TestCase("f2 = 1",      false)]
        [TestCase("this.f2 = 1", true)]
        [TestCase("F3 = 1",      false)]
        [TestCase("this.F3 = 1", true)]
        public static async Task AssigningInCtor(string expression, bool expected)
        {
            var sln = CodeFactory.CreateSolution(@"
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

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
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
            Assert.AreEqual(null, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", true)]
        [TestCase("f", false)]
        public static async Task ReturningInMethodExpressionBody(string expression, bool expected)
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

        [TestCase("this.f", true)]
        [TestCase("f",      false)]
        public static async Task ReturningInPropertyExpressionBody(string expression, bool expected)
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    class C
    {
        private int f;

        public int P => this.f;
    }
}".AssertReplace("this.f", expression));

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", true)]
        [TestCase("f",      false)]
        public static async Task ReturningInPropertyGetterExpressionBody(string expression, bool expected)
        {
            var sln = CodeFactory.CreateSolution(@"
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

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f", true)]
        [TestCase("f",      false)]
        public static async Task ReturningInPropertyGetterStatementBody(string expression, bool expected)
        {
            var sln = CodeFactory.CreateSolution(@"
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

            var document = sln.Projects.Single().Documents.Single();
            Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("this.f = 1", true)]
        [TestCase("f = 1", false)]
        [TestCase("", null)]
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
                Assert.AreEqual(expected, await CodeStyle.QualifyFieldAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
            }
        }
    }
}

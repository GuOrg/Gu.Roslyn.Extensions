namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class QuyalifyMethodAccess
    {
        [Test]
        public static async Task DefaultsToTrue()
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
            Assert.AreEqual(true, await CodeStyle.QualifyMethodAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("M1()", false)]
        [TestCase("this.M1()", true)]
        [TestCase("M2()", true)]
        public static async Task CallInCtor(string expression, bool expected)
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
            Assert.AreEqual(expected, await CodeStyle.QualifyMethodAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("M1()", false)]
        [TestCase("this.M1()", true)]
        [TestCase("M2()", true)]
        public static async Task ExpressionBody(string expression, bool expected)
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
            Assert.AreEqual(expected, await CodeStyle.QualifyMethodAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("M1()",      false)]
        [TestCase("this.M1()", true)]
        [TestCase("M2()",      true)]
        public static async Task Assignment(string expression, bool expected)
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
            Assert.AreEqual(expected, await CodeStyle.QualifyMethodAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }

        [TestCase("M1()",      false)]
        [TestCase("this.M1()", true)]
        [TestCase("M2()",      true)]
        public static async Task Argument(string expression, bool expected)
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
            Assert.AreEqual(expected, await CodeStyle.QualifyMethodAccessAsync(document, CancellationToken.None).ConfigureAwait(false));
        }
    }
}

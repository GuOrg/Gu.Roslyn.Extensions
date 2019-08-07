namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UnderscoreFields
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static void DefaultsToNull()
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
            var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(sln.Projects.Single().Documents.Single()).Result;
            Assert.AreEqual(null, CodeStyle.UnderscoreFields(editor));
        }

        [TestCase("private int _f",              true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f",               false)]
        [TestCase("private readonly int f",      false)]
        public static void FiguresOutFromDocument(string declaration, bool expected)
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
                var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(document).Result;
                Assert.AreEqual(expected, CodeStyle.UnderscoreFields(editor));
            }
        }

        [TestCase("private int _f",              true)]
        [TestCase("private readonly int _f = 1", true)]
        [TestCase("private int f",               false)]
        [TestCase("private readonly int f",      false)]
        public static void FiguresOutFromOtherDocument(string declaration, bool expected)
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
                var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(document).Result;
                Assert.AreEqual(expected, CodeStyle.UnderscoreFields(editor));
            }
        }
    }
}

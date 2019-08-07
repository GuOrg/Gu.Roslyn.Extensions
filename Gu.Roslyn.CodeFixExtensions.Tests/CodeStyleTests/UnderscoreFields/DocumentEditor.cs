namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UnderscoreFields
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static void WhenUnknown()
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
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.UnderscoreFields(editor));
        }

        [TestCase("private int _f",              CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f",               CodeStyleResult.No)]
        [TestCase("private readonly int f",      CodeStyleResult.No)]
        public static void FiguresOutFromDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(new[]
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

        [TestCase("private int _f",              CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f",               CodeStyleResult.No)]
        [TestCase("private readonly int f",      CodeStyleResult.No)]
        public static void FiguresOutFromOtherDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(new[]
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

namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.BackingFieldsAdjacent
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static void WhenUnknown()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
    }
}");
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(editor, out _));
        }

        [Test]
        public static void WhenUnknownTwoDocuments()
        {
            var syntaxTree1 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C1
    {
    }
}");

            var syntaxTree2 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C2
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree1, syntaxTree2 });
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree1), out _));
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree2), out _));
        }

        [Test]
        public static void WhenUnknownOneProperty()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f1;

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(editor, out _));
        }

        [Test]
        public static void WhenUnknownOnePropertyFieldAbove()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f;
        private int f1;

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(editor, out _));
        }

        [Test]
        public static void OnePropertyCtorBetween()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f1;

        public C()
        {
        }

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(editor, out _));
        }

        [Test]
        public static void OnePropertyCtorAbove()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int f1;

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(editor, out var newLineBetween));
            Assert.AreEqual(true, newLineBetween);
        }

        [Test]
        public static void TwoProperties()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f1;

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }

        private int f2;

        public int P2
        {
            get => this.f2;
            set => this.f2 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(editor, out var newLineBetween));
            Assert.AreEqual(true, newLineBetween);
        }

        [Test]
        public static void TwoExpressionBodyProperties()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f1;
        public int P1 => this.f1;

        private int f2;
        public int P2 => this.f2;
    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(editor, out var newLineBetween));
            Assert.AreEqual(false, newLineBetween);
        }

        [Test]
        public static void WhenStyleCop()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        private int f1;

        public C()
        {
        }

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(editor, out _));
        }

        [Test]
        public static void WhenAdjacentNoEmptyLine()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int f1;
        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(editor, out var newLineBetween));
            Assert.AreEqual(false, newLineBetween);
        }

        [Test]
        public static void WhenAdjacentWithEmptyLine()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    public class C
    {
        public C()
        {
        }

        private int f1;

        public int P1
        {
            get => this.f1;
            set => this.f1 = value;
        }
    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(editor, out var newLineBetween));
            Assert.AreEqual(true, newLineBetween);
        }

        private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
        {
            return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
        }
    }
}

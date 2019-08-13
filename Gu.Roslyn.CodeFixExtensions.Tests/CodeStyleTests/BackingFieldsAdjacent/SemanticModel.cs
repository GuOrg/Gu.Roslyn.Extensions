namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.BackingFieldsAdjacent
{
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class SemanticModel
    {
        [Test]
        public static void WhenUnknown()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
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
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
        }

        [Test]
        public static void WhenUnknownOnePropertyFieldAbove()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
        }

        [Test]
        public static void OnePropertyCtorBetween()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree), out _));
        }

        [Test]
        public static void OnePropertyCtorAbove()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree), out _));
        }

        [Test]
        public static void TwoProperties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree), out _));
        }

        [Test]
        public static void TwoExpressionBodyProperties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree), out var newLineBetween));
            Assert.AreEqual(false, newLineBetween);
        }

        [Test]
        public static void FindsAdjacentInCompilation()
        {
            var syntaxTree1 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
    }
}");

            var syntaxTree2 = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree1, syntaxTree2 });
            var semanticModel = compilation.GetSemanticModel(syntaxTree1);
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(semanticModel, out var newLine));
            Assert.AreEqual(true, newLine);
        }

        [Test]
        public static void FindsInOtherDocument()
        {
            var syntaxTree1 = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
    }
}");

            var syntaxTree2 = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree1, syntaxTree2 });
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree1), out _));
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree2), out _));
        }

        [Test]
        public static void WhenStyleCop()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
        }

        [Test]
        public static void WhenAdjacentNoEmptyLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(semanticModel, out var newLineBetween));
            Assert.AreEqual(false, newLineBetween);
        }

        [Test]
        public static void WhenAdjacentWithEmptyLine()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.BackingFieldsAdjacent(compilation.GetSemanticModel(syntaxTree), out var newLineBetween));
            Assert.AreEqual(true, newLineBetween);
        }
    }
}

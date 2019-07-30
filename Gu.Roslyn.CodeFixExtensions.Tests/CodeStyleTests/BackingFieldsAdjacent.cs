namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class BackingFieldsAdjacent
    {
        [Test]
        public static void DefaultsToStyleCopEmptyClass()
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
            Assert.AreEqual(false, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
        }

        [Test]
        public static void DefaultsToStyleCopWhenOneProperty()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
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

        private int value1;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree1, syntaxTree2 });
            var semanticModel = compilation.GetSemanticModel(syntaxTree1);
            Assert.AreEqual(true, CodeStyle.BackingFieldsAdjacent(semanticModel, out var newLine));
            Assert.AreEqual(true, newLine);
        }

        [Test]
        public static void FindsStyleCopInCompilation()
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
        private int value1;

        public C()
        {
        }

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree1, syntaxTree2 });
            var semanticModel = compilation.GetSemanticModel(syntaxTree1);
            Assert.AreEqual(false, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
        }

        [Test]
        public static void WhenStyleCop()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value1;

        public C()
        {
        }

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.BackingFieldsAdjacent(semanticModel, out _));
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

        private int value1;
        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.BackingFieldsAdjacent(semanticModel, out var newLineBetween));
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

        private int value1;

        public int Value1
        {
            get => this.value1;
            set => this.value1 = value;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.BackingFieldsAdjacent(semanticModel, out var newLineBetween));
            Assert.AreEqual(true, newLineBetween);
        }
    }
}

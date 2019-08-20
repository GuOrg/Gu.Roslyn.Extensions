namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.ExecutionWalkerTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class WhenProperty
    {
        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "1, 2, 3")]
        [TestCase(SearchScope.Type, "1, 2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void StatementBody(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value
        {
            get { return 1; }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "2, 3")]
        [TestCase(SearchScope.Type, "1, 2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void Static(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(Value, 2);
            int j = 3;
        }

        public static int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "2, 3")]
        [TestCase(SearchScope.Type, "2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void StaticOtherType(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public static class C1
    {
        public static int Value => 1;
    }

    public class C2
    {
        public C2()
        {
            Equals(C1.Value, 2);
            int j = 3;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "1, 2, 3")]
        [TestCase(SearchScope.Type, "1, 2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void ExpressionBody(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "1, 2, 3")]
        [TestCase(SearchScope.Type, "1, 2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void ExpressionBodyGetter(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value
        {
            get => 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "")]
        [TestCase(SearchScope.Instance, "1, 2")]
        [TestCase(SearchScope.Type, "1, 2")]
        [TestCase(SearchScope.Recursive, "1, 2")]
        public static void AssignmentSetterWithGetter(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            Value = Value;
        }

        public int Value
        {
            get => 1;
            set => value = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "")]
        [TestCase(SearchScope.Instance, "1, 2")]
        [TestCase(SearchScope.Type, "1, 2")]
        [TestCase(SearchScope.Recursive, "1, 2")]
        public static void AssignmentSetterWithGetterThis(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            this.Value = this.Value;
        }

        public int Value
        {
            get => 1;
            set => this.value = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "")]
        [TestCase(SearchScope.Instance, "1, 2")]
        [TestCase(SearchScope.Type, "1, 2")]
        [TestCase(SearchScope.Recursive, "1, 2")]
        public static void PropertyUnary(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private int value;

        public C()
        {
            Value++;
        }

        public int Value
        {
            get => 1;
            set => value = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase("Value1 > Value2", SearchScope.Member, "")]
        [TestCase("Value1 > Value2", SearchScope.Instance, "1, 2")]
        [TestCase("Value1 > Value2", SearchScope.Type, "1, 2")]
        [TestCase("Value1 > Value2", SearchScope.Recursive, "1, 2")]
        public static void BinaryCompare(string expression, SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int Value1 => 1;

        public int Value2 => 2;

        public bool M() => Value1 > Value2;
    }
}".AssertReplace("Value1 > Value2", expression));
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("M");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase("Value1 ?? Value2", SearchScope.Member, "")]
        [TestCase("Value1 ?? Value2", SearchScope.Instance, "1, 2")]
        [TestCase("Value1 ?? Value2", SearchScope.Type, "1, 2")]
        [TestCase("Value1 ?? Value2", SearchScope.Recursive, "1, 2")]
        [TestCase("(int)Value1 > (int)Value2", SearchScope.Member, "")]
        [TestCase("(int)Value1 > (int)Value2", SearchScope.Instance, "1, 2")]
        [TestCase("(int)Value1 > (int)Value2", SearchScope.Type, "1, 2")]
        [TestCase("(int)Value1 > (int)Value2", SearchScope.Recursive, "1, 2")]
        public static void Binary(string expression, SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public object Value1 => 1;

        public object Value2 => 2;

        public object M() => Value1 ?? Value2;
    }
}".AssertReplace("Value1 ?? Value2", expression));
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("M");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(SearchScope.Member, "2, 3")]
        [TestCase(SearchScope.Instance, "1, 2, 3")]
        [TestCase(SearchScope.Type, "1, 2, 3")]
        [TestCase(SearchScope.Recursive, "1, 2, 3")]
        public static void ExpressionBodyAsArgument(SearchScope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }
    }
}

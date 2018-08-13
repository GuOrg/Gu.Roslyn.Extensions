namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class ExecutionWalkerTests
    {
        public class Property
        {
            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void StatementBody(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
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
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void Static(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            Equals(Value, 2);
            int j = 3;
        }

        public static int Value => 1;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "2, 3")]
            [TestCase(Scope.Type, "2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void StaticOtherType(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public static class Bar
    {
        public static int Value => 1;
    }

    public class Foo
    {
        public Foo()
        {
            Equals(Bar.Value, 2);
            int j = 3;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ExpressionBody(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value => 1;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ExpressionBodyGetter(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
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
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void AssignmentSetterWithGetter(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public Foo()
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
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void AssignmentSetterWithGetterThis(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public Foo()
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
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void PropertyUnary(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value;

        public Foo()
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
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase("Value1 > Value2", Scope.Member, "")]
            [TestCase("Value1 > Value2", Scope.Instance, "1, 2")]
            [TestCase("Value1 > Value2", Scope.Type, "1, 2")]
            [TestCase("Value1 > Value2", Scope.Recursive, "1, 2")]
            public void BinaryCompare(string expression, Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1 => 1;

        public int Value2 => 2;

        public bool Bar() => Value1 > Value2;
    }
}".AssertReplace("Value1 > Value2", expression));
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration("Bar");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase("Value1 ?? Value2", Scope.Member, "")]
            [TestCase("Value1 ?? Value2", Scope.Instance, "1, 2")]
            [TestCase("Value1 ?? Value2", Scope.Type, "1, 2")]
            [TestCase("Value1 ?? Value2", Scope.Recursive, "1, 2")]
            [TestCase("(int)Value1 > (int)Value2", Scope.Member, "")]
            [TestCase("(int)Value1 > (int)Value2", Scope.Instance, "1, 2")]
            [TestCase("(int)Value1 > (int)Value2", Scope.Type, "1, 2")]
            [TestCase("(int)Value1 > (int)Value2", Scope.Recursive, "1, 2")]
            public void Binary(string expression, Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public object Value1 => 1;

        public object Value2 => 2;

        public object Bar() => Value1 ?? Value2;
    }
}".AssertReplace("Value1 ?? Value2", expression));
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration("Bar");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ExpressionBodyAsArgument(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            Equals(this.Value, 2);
            int j = 3;
        }

        public int Value => 1;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }
        }
    }
}

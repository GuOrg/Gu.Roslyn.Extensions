namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class ExecutionWalkerTests
    {
        public class Invocation
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
            Equals(this.Value(), 2);
            int j = 3;
        }

        public int Value()
        {
            return 1;
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
            Equals(Value(), 2);
            int j = 3;
        }

        public static int Value()
        {
            return 1;
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
            [TestCase(Scope.Type, "2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void StaticOtherType(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public static class Bar
    {
        public static int Value() => 1;
    }

    public class Foo
    {
        public Foo()
        {
            Equals(Bar.Value(), 2);
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
            Equals(this.Value(), 2);
            int j = 3;
        }

        public int Value() => 1;
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

            [TestCase(Scope.Member, "1")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void WalkOverridden(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class FooBase
    {
        protected virtual int Bar() => 2;
    }

    public sealed class Foo : FooBase
    {
        protected override int Bar() => 1 * base.Bar();
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration("protected override int Bar()");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void InvocationAsArgument(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            Equals(this.Value(), 2);
            int j = 3;
        }

        public int Value() => 1;
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

            [TestCase(Scope.Member, "1, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void InvocationVirtual(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo : IDisposable
    {
        public void Dispose()
        {
            var i = 1;
            Dispose(true);
            i = 3;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var j = 2;
            }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration("public void Dispose()");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ArgumentBeforeInvocation(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var value = this.Meh(this.Value());
            value = 3;
        }

        public int Value() => 1;

        private int Meh(int i)
        {
            return 2 * i;
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

            [TestCase(Scope.Member, "3")]
            [TestCase(Scope.Instance, "1, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ArgumentBeforeInvocationStaticAndInstance(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var value = Meh(this.Value());
            value = 3;
        }

        public int Value() => 1;

        private static int Meh(int i)
        {
            return 2 * i;
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
        }
    }
}

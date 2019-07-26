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
namespace N
{
    public class C
    {
        public C()
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
                var node = syntaxTree.FindConstructorDeclaration("C");
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
namespace N
{
    public class C
    {
        public C()
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
                var node = syntaxTree.FindConstructorDeclaration("C");
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
namespace N
{
    public static class Bar
    {
        public static int Value() => 1;
    }

    public class C
    {
        public C()
        {
            Equals(Bar.Value(), 2);
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

            [TestCase(Scope.Member, "2, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ExpressionBody(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value(), 2);
            int j = 3;
        }

        public int Value() => 1;
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

            [TestCase(Scope.Member, "1")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void WalkOverridden(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class CBase
    {
        protected virtual int Bar() => 2;
    }

    public sealed class C : CBase
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
namespace N
{
    public class C
    {
        public C()
        {
            Equals(this.Value(), 2);
            int j = 3;
        }

        public int Value() => 1;
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

            [TestCase(Scope.Member, "1, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void InvocationVirtual(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;

    public class C : IDisposable
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
namespace N
{
    public class C
    {
        public C()
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
                var node = syntaxTree.FindConstructorDeclaration("C");
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
namespace N
{
    public class C
    {
        public C()
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
                var node = syntaxTree.FindConstructorDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }
        }
    }
}

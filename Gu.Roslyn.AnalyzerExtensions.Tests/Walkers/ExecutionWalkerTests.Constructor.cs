namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class ExecutionWalkerTests
    {
        public class Constructor
        {
            [TestCase(Scope.Member)]
            [TestCase(Scope.Instance)]
            [TestCase(Scope.Type)]
            [TestCase(Scope.Recursive)]
            public void ExplicitParameterless(Scope scope)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual("1", walker.Literals.Single().ToString());
                }
            }

            [TestCase(Scope.Member, "1, 2")]
            [TestCase(Scope.Instance, "2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void StaticBeforeExplicitParameterless(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        static C()
        {
            var i = 1;
        }

        public C()
        {
            var i = 2;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindTypeDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "1, 2")]
            [TestCase(Scope.Instance, "2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void StaticBeforeExplicitParameterlessWhenNotDocumentOrder(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 2;
        }

        static C()
        {
            var i = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindTypeDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "1, 3")]
            [TestCase(Scope.Instance, "1, 2, 3")]
            [TestCase(Scope.Type, "1, 2, 3")]
            [TestCase(Scope.Recursive, "1, 2, 3")]
            public void ChainedThis(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
            : this(1)
        {
            var j = 3;
        }

        public C(int _)
        {
            var i = 2;
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

            [TestCase(Scope.Member, "2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void ImplicitBaseParameterLess(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class CBase
    {
        public CBase()
        {
            var i = 1;
        }
    }

    public class C : CBase
    {
        public C()
        {
            var j = 2;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("public C()");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void ExplicitBaseParameterLess(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class CBase
    {
        public CBase()
        {
            var i = 1;
        }
    }

    public class C : CBase
    {
        public C()
            : base()
        {
            var j = 2;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("public C()");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "1, 2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void FieldInitializerBeforeConstructor(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private readonly int value = 1;

        public C()
        {
            this.value = 2;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindClassDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "1, 2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void FieldInitializerBeforeConstructorWhenNotDocumentOrder(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            this.value = 2;
        }

        private readonly int value = 1;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindClassDeclaration("C");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void PropertyInitializerBeforeConstructor(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C() { Value2 = 2 };

        public C()
        {
        }

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindExpression("new C() { Value2 = 2 }");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void PropertyInitializerBeforeDefaultCtor(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C() { Value2 = 2 };

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindExpression("new C() { Value2 = 2 }");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }

            [TestCase(Scope.Member, "2")]
            [TestCase(Scope.Instance, "1, 2")]
            [TestCase(Scope.Type, "1, 2")]
            [TestCase(Scope.Recursive, "1, 2")]
            public void PropertyInitializerBeforeDefaultCtorObjectInitializer(Scope scope, string expected)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C { Value2 = 2 };

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindExpression("new C { Value2 = 2 }");
                using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual(expected, string.Join(", ", walker.Literals));
                }
            }
        }
    }
}

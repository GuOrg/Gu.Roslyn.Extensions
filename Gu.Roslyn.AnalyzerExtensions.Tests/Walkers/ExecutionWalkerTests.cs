namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class ExecutionWalkerTests
    {
        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Recursive)]
        public void SimpleCtor(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var i = 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("Foo");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("1", walker.Literals.Single().ToString());
            }
        }

        [TestCase(Scope.Member, "1, 3")]
        [TestCase(Scope.Instance, "1, 2, 3")]
        [TestCase(Scope.Recursive, "1, 2, 3")]
        public void ChainedCtor(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
            : this(1)
        {
            var j = 3;
        }

        public Foo(int _)
        {
            var i = 2;
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

        [TestCase(Scope.Member, "2")]
        [TestCase(Scope.Instance, "1, 2")]
        [TestCase(Scope.Recursive, "1, 2")]
        public void ImplicitBaseCtor(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class FooBase
    {
        public FooBase()
        {
            var i = 1;
        }
    }

    public class Foo : FooBase
    {
        public Foo()
        {
            var j = 2;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("public Foo()");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.Literals));
            }
        }

        private class LiteralWalker : ExecutionWalker<LiteralWalker>
        {
            private readonly List<LiteralExpressionSyntax> literals = new List<LiteralExpressionSyntax>();

            public IReadOnlyList<LiteralExpressionSyntax> Literals => this.literals;

            public static LiteralWalker Borrow(SyntaxNode node, Scope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                return BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new LiteralWalker());
            }

            public override void VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                this.literals.Add(node);
                base.VisitLiteralExpression(node);
            }

            protected override void Clear()
            {
                this.literals.Clear();
                base.Clear();
            }
        }
    }
}

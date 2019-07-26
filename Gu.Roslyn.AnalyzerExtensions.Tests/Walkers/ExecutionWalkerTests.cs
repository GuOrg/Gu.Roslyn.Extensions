namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public partial class ExecutionWalkerTests
    {
        [TestCase(Scope.Member, "2")]
        [TestCase(Scope.Instance, "1, 2")]
        [TestCase(Scope.Type, "1, 2")]
        [TestCase(Scope.Recursive, "1, 2")]
        public void LocalDeclarationWithExpressionBody(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var value = this.Value;
            value = 2;
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

        [TestCase(Scope.Member, "2")]
        [TestCase(Scope.Instance, "1, 2")]
        [TestCase(Scope.Type, "1, 2")]
        [TestCase(Scope.Recursive, "1, 2")]
        public void LocalDeclarationWithCastExpressionBody(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var value = (double)this.Value;
            value = 2;
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

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Type)]
        [TestCase(Scope.Recursive)]
        public void IgnoreNameOfProperty(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var text = nameof(this.Value);
        }

        public int Value => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(string.Empty, string.Join(", ", walker.Literals));
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Type)]
        [TestCase(Scope.Recursive)]
        public void IgnoreNameOfMethod(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var text = nameof(this.Value());
        }

        public int Value() => 1;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindConstructorDeclaration("C");
            using (var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(string.Empty, string.Join(", ", walker.Literals));
            }
        }

        private class LiteralWalker : ExecutionWalker<LiteralWalker>
        {
            private readonly List<LiteralExpressionSyntax> literals = new List<LiteralExpressionSyntax>();

            internal IReadOnlyList<LiteralExpressionSyntax> Literals => this.literals;

            public override void VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                if (node.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    this.literals.Add(node);
                }

                base.VisitLiteralExpression(node);
            }

            internal static LiteralWalker Borrow(SyntaxNode node, Scope scope, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                return BorrowAndVisit(node, scope, semanticModel, cancellationToken, () => new LiteralWalker());
            }

            protected override void Clear()
            {
                this.literals.Clear();
                base.Clear();
            }
        }
    }
}

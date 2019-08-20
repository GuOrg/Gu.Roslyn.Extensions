namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class SyntaxNodeExtTests
    {
        [TestCase("\"abc\"", true)]
        [TestCase("1", true)]
        [TestCase("2", true)]
        [TestCase("3", false)]
        [TestCase("4", true)]
        [TestCase("5", false)]
        [TestCase("6", true)]
        [TestCase("7", true)]
        [TestCase("8", true)]
        [TestCase("9", false)]
        public static void IsInStaticContext(string expression, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    [Obsolete(""abc"")]
    public class C
    {
        private readonly int f = 1;

        public C()
            :this(2)
        {
        }

        public C(int _)
        {
            this.f = 3;
        }

        public static int P1 => 4;

        public int P2 => 5;

        public static int P3 { get; } = 6;

        public int P4 { get; } = 7;

        public static int M() => 8;

        public int M() => 9;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(expected, node.IsInStaticContext());
        }

        [TestCase("1", true)]
        [TestCase("M", false)]
        public static void WhenReturningExpression(string text, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static Expression<Func<int>> M() => () => 1;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<SyntaxNode>(text);
            Assert.AreEqual(expected, node.IsInExpressionTree(semanticModel, CancellationToken.None));
        }

        [TestCase("1", true)]
        [TestCase("M", false)]
        public static void WhenExpressionArgument(string text, bool expected)
        {
            var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static void M() => M(() => 1);

        public static void M(Expression<Func<int>> _) { }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<SyntaxNode>(text);
            Assert.AreEqual(expected, node.IsInExpressionTree(semanticModel, CancellationToken.None));
        }

        [TestCase("1")]
        [TestCase("M")]
        public static void WhenFunc(string text)
        {
            var code = @"
namespace N
{
    using System;

    public class C
    {
        public static Func<int> M() => () => 1;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<SyntaxNode>(text);
            Assert.AreEqual(false, node.IsInExpressionTree(semanticModel, CancellationToken.None));
        }

        [TestCase("1")]
        [TestCase("\abc\".Length")]
        public static void WhenOtherKindArgument(string expression)
        {
            var code = @"
namespace N
{
    using System;
    using System.Linq.Expressions;

    public class C
    {
        public static void M() => M(1);

        public static void M(int _) { }
    }
}".AssertReplace("1", expression);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<SyntaxNode>(expression);
            Assert.AreEqual(false, node.IsInExpressionTree(semanticModel, CancellationToken.None));
        }
    }
}

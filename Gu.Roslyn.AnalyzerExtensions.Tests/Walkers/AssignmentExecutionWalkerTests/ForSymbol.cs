namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.AssignmentExecutionWalkerTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class ForSymbol
    {
        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Recursive)]
        public static void FieldWithCtorArg(Scope scope)
        {
            var code = @"
namespace N
{
    internal class C
    {
        private readonly int value;

        internal C(int arg)
        {
            this.value = arg;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var value = syntaxTree.FindMemberAccessExpression("this.value");
            var ctor = syntaxTree.FindConstructorDeclaration("C(int arg)");
            var field = semanticModel.GetSymbolSafe(value, CancellationToken.None);
            Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out var result));
            Assert.AreEqual("this.value = arg", result.ToString());
            Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
            Assert.AreEqual("this.value = arg", result.ToString());
            using (var walker = AssignmentExecutionWalker.For(field, ctor, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("this.value = arg", walker.Assignments.Single().ToString());
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Recursive)]
        public static void FieldWithChainedCtorArg(Scope scope)
        {
            var code = @"
namespace N
{
    internal class C
    {
        private readonly int value;

        public C()
            : this(1)
        {
        }

        internal C(int arg)
        {
            this.value = arg;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var value = syntaxTree.FindMemberAccessExpression("this.value");
            var ctor = syntaxTree.FindConstructorDeclaration("C()");
            AssignmentExpressionSyntax result;
            var field = semanticModel.GetSymbolSafe(value, CancellationToken.None);
            if (scope != Scope.Member)
            {
                Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.value = arg", result.ToString());
                Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.value = arg", result.ToString());
                using (var walker = AssignmentExecutionWalker.For(field, ctor, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual("this.value = arg", walker.Assignments.Single().ToString());
                }
            }
            else
            {
                Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out _));
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Recursive)]
        public static void FieldPrivateCtorCalledByInitializer(Scope scope)
        {
            var code = @"
namespace N
{
    internal class C
    {
        public static readonly C Default = new C();
        private readonly int value;

        private C()
        {
            this.value = 1;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var field = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindFieldDeclaration("private readonly int value"), CancellationToken.None);
            var bar = syntaxTree.FindTypeDeclaration("C");
            Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, bar, scope, semanticModel, CancellationToken.None, out var result));
            Assert.AreEqual("this.value = 1", result.ToString());
            Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, bar, scope, semanticModel, CancellationToken.None, out result));
            Assert.AreEqual("this.value = 1", result.ToString());
            using (var walker = AssignmentExecutionWalker.For(field, bar, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual("this.value = 1", walker.Assignments.Single().ToString());
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Recursive)]
        public static void FieldWithCtorArgViaProperty(Scope scope)
        {
            var code = @"
namespace N
{
    internal class C
    {
        private int number;

        internal C(int arg)
        {
            this.Number = arg;
        }

        public int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var value = syntaxTree.FindMemberAccessExpression("this.number");
            var ctor = syntaxTree.FindConstructorDeclaration("C(int arg)");
            AssignmentExpressionSyntax result;
            var field = semanticModel.GetSymbolSafe(value, CancellationToken.None);
            if (scope != Scope.Member)
            {
                Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.number = value", result.ToString());
                Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.number = value", result.ToString());
                using (var walker = AssignmentExecutionWalker.For(field, ctor, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual("this.number = value", walker.Assignments.Single().ToString());
                }
            }
            else
            {
                Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out _));
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Type)]
        [TestCase(Scope.Recursive)]
        public static void FieldInPropertyExpressionBody(Scope scope)
        {
            var code = @"
namespace N
{
    internal class C
    {
        private int number;

        internal C()
        {
            var i = this.Number;
        }

        public int Number => this.number = 3;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var value = syntaxTree.FindMemberAccessExpression("this.number");
            var ctor = syntaxTree.FindConstructorDeclaration("C()");
            AssignmentExpressionSyntax result;
            var field = semanticModel.GetSymbolSafe(value, CancellationToken.None);
            if (scope != Scope.Member)
            {
                Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.number = 3", result.ToString());
                Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                Assert.AreEqual("this.number = 3", result.ToString());
                using (var walker = AssignmentExecutionWalker.For(field, ctor, scope, semanticModel, CancellationToken.None))
                {
                    Assert.AreEqual("this.number = 3", walker.Assignments.Single().ToString());
                }
            }
            else
            {
                Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out _));
            }
        }
    }
}

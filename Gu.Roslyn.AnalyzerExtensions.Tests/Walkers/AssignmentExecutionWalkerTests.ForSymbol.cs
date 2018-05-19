namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using NUnit.Framework;

    public partial class AssignmentExecutionWalkerTests
    {
        internal class ForSymbol
        {
            [TestCase(Scope.Member)]
            [TestCase(Scope.Instance)]
            [TestCase(Scope.Recursive)]
            public void FieldWithCtorArg(Scope scope)
            {
                var testCode = @"
namespace RoslynSandbox
{
    internal class Foo
    {
        private readonly int value;

        internal Foo(int arg)
        {
            this.value = arg;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindMemberAccessExpression("this.value");
                var ctor = syntaxTree.FindConstructorDeclaration("Foo(int arg)");
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
            public void FieldWithChainedCtorArg(Scope scope)
            {
                var testCode = @"
namespace RoslynSandbox
{
    internal class Foo
    {
        private readonly int value;

        public Foo()
            : this(1)
        {
        }

        internal Foo(int arg)
        {
            this.value = arg;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindMemberAccessExpression("this.value");
                var ctor = syntaxTree.FindConstructorDeclaration("Foo()");
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
                    Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                }
            }

            [TestCase(Scope.Member)]
            [TestCase(Scope.Instance)]
            [TestCase(Scope.Recursive)]
            public void FieldPrivateCtorCalledByInitializer(Scope scope)
            {
                var testCode = @"
namespace RoslynSandbox
{
    internal class Foo
    {
        public static readonly Foo Default = new Foo();
        private readonly int value;

        private Foo()
        {
            this.value = 1;
        }

        public void Bar()
        {
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                AssignmentExpressionSyntax result;
                var field = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindFieldDeclaration("private readonly int value"), CancellationToken.None);
                var bar = syntaxTree.FindMethodDeclaration("Bar()");
                if (scope != Scope.Member)
                {
                    Assert.AreEqual(true, AssignmentExecutionWalker.FirstFor(field, bar, scope, semanticModel, CancellationToken.None, out result));
                    Assert.AreEqual("this.value = 1", result.ToString());
                    Assert.AreEqual(true, AssignmentExecutionWalker.SingleFor(field, bar, scope, semanticModel, CancellationToken.None, out result));
                    Assert.AreEqual("this.value = 1", result.ToString());
                    using (var walker = AssignmentExecutionWalker.For(field, bar, scope, semanticModel, CancellationToken.None))
                    {
                        Assert.AreEqual("this.value = 1", walker.Assignments.Single().ToString());
                    }
                }
                else
                {
                    Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, bar, scope, semanticModel, CancellationToken.None, out result));
                }
            }

            [TestCase(Scope.Member)]
            [TestCase(Scope.Instance)]
            [TestCase(Scope.Recursive)]
            public void FieldWithCtorArgViaProperty(Scope scope)
            {
                var testCode = @"
namespace RoslynSandbox
{
    internal class Foo
    {
        private int number;

        internal Foo(int arg)
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
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindMemberAccessExpression("this.number");
                var ctor = syntaxTree.FindConstructorDeclaration("Foo(int arg)");
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
                    Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                }
            }

            [TestCase(Scope.Member)]
            [TestCase(Scope.Instance)]
            [TestCase(Scope.Recursive)]
            public void FieldInPropertyExpressionBody(Scope scope)
            {
                var testCode = @"
namespace RoslynSandbox
{
    internal class Foo
    {
        private int number;

        internal Foo()
        {
            var i = this.Number;
        }

        public int Number => this.number = 3;
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var value = syntaxTree.FindMemberAccessExpression("this.number");
                var ctor = syntaxTree.FindConstructorDeclaration("Foo()");
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
                    Assert.AreEqual(false, AssignmentExecutionWalker.FirstFor(field, ctor, scope, semanticModel, CancellationToken.None, out result));
                }
            }
        }
    }
}

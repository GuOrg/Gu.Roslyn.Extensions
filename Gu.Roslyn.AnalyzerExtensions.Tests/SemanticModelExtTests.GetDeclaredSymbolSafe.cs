namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class SemanticModelExtTests
    {
        public class GetDeclaredSymbolSafe
        {
            [Test]
            public void GetDeclaredSymbolSafField()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    public class Foo
    {
        private readonly int bar;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] {syntaxTree});
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindFieldDeclaration("bar");
                Assert.AreEqual(
                    semanticModel.GetDeclaredSymbol(node.Declaration.Variables[0]),
                    semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafConstructor()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] {syntaxTree});
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                Assert.AreEqual(
                    semanticModel.GetDeclaredSymbol(node),
                    semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeEvent()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler Bar;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] {syntaxTree});
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindBestMatch<EventFieldDeclarationSyntax>("Bar");
                Assert.AreEqual(
                    semanticModel.GetDeclaredSymbol(node),
                    semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeProperty()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        public int Bar { get; set; }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] {syntaxTree});
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindPropertyDeclaration("Bar");
                Assert.AreEqual(
                    semanticModel.GetDeclaredSymbol(node),
                    semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeMethod()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal void Bar()
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] {syntaxTree});
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration("Bar");
                Assert.AreEqual(
                    semanticModel.GetDeclaredSymbol(node),
                    semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }
        }
    }
}

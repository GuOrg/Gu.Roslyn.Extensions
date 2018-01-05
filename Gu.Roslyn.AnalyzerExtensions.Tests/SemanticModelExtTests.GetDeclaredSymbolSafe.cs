namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class SemanticModelExtTests
    {
        public class GetDeclaredSymbolSafe
        {
            private static readonly SyntaxTree OtherTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        private readonly int bar;
    }
}");

            [Test]
            public void GetDeclaredSymbolSafeType()
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindClassDeclaration("Foo");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeField()
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindFieldDeclaration("bar");
                var expected = semanticModel.GetDeclaredSymbol(node.Declaration.Variables[0]);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeConstructor()
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindConstructorDeclaration("Foo");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeParameter()
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindBestMatch<ParameterSyntax>("i");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeVariable()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            var i = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindBestMatch<VariableDeclarationSyntax>("i");
                var expected = semanticModel.GetDeclaredSymbol(node.Variables[0]);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindBestMatch<EventFieldDeclarationSyntax>("Bar");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindPropertyDeclaration("Bar");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe((BasePropertyDeclarationSyntax)node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void GetDeclaredSymbolSafeIndexer()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        public int this[int index]
        {
            get { return index; }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindBestMatch<IndexerDeclarationSyntax>("this");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindMethodDeclaration("Bar");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }
        }
    }
}

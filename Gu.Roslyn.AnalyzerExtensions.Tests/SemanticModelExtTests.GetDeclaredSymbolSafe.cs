namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public partial class SemanticModelExtTests
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
            public void ClassDeclaration()
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
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void FieldDeclaration()
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
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void ConstructorDeclaration()
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
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void EventDeclaration()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler Bar
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindEventDeclaration("Bar");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void EventFieldDeclaration()
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
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindEventFieldDeclaration("Bar");
                var expected = semanticModel.GetDeclaredSymbol(node.Declaration.Variables[0]);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void PropertyDeclaration()
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
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void IndexerDeclaration()
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
                var node = syntaxTree.FindIndexerDeclaration("this");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void MethodDeclaration()
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

            [Test]
            public void Parameter()
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
                var node = syntaxTree.FindParameter("i");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void VariableDeclaration()
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
                var node = syntaxTree.FindVariableDeclaration("i");
                var expected = semanticModel.GetDeclaredSymbol(node.Variables[0]);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void VariableDesignation()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo()
        {
            int.TryParse(string.Empty, out var i);
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.Find<VariableDesignationSyntax>("out var i");
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }
        }
    }
}

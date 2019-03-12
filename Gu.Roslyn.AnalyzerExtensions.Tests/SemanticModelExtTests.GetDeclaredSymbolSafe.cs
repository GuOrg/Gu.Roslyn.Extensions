namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Linq;
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
    public class C
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
    public class C
    {
        private readonly int bar;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindClassDeclaration("C");
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
    public class C
    {
        private readonly int f;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindFieldDeclaration("f");
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
    internal class C
    {
        internal C(int i, double d)
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindConstructorDeclaration("C");
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

    public class C
    {
        public event EventHandler E
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindEventDeclaration("E");
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

    public class C
    {
        public event EventHandler E;
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindEventFieldDeclaration("E");
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
    internal class C
    {
        public int P { get; set; }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindPropertyDeclaration("P");
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
    internal class C
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
            public void IndexerWhenArrayElementAccess()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    class C
    {
        C(int[] xs)
        {
            xs[0] = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = (ElementAccessExpressionSyntax)syntaxTree.FindAssignmentExpression("xs[0] = 1").Left;
                var expected = (IArrayTypeSymbol)semanticModel.GetTypeInfo(node.Expression).Type;
                Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
                Assert.AreEqual(expected, symbol);
            }

            [Test]
            public void IndexerWhenMultidimensionalArrayElementAccess()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    class C
    {
        C(int[,] xs)
        {
            xs[0, 0] = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = (ElementAccessExpressionSyntax)syntaxTree.FindAssignmentExpression("xs[0, 0] = 1").Left;
                var expected = (IArrayTypeSymbol)semanticModel.GetTypeInfo(node.Expression).Type;
                Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true,     semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
                Assert.AreEqual(expected, symbol);
            }

            [Test]
            public void IndexerWhenListElementAccess()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

    class C
    {
        C(List<int> xs)
        {
            xs[0] = 1;
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = (ElementAccessExpressionSyntax)syntaxTree.FindAssignmentExpression("xs[0] = 1").Left;
                var expected = (IPropertySymbol)semanticModel.GetTypeInfo(node.Expression).Type.GetMembers().Single(x => x is IPropertySymbol property && property.IsIndexer);
                Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var indexer));
                Assert.AreEqual(expected, indexer);
            }

            [Test]
            public void IndexerWhenDictionaryElementAccess()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

     class C
    {
        C(Dictionary<int, string> map)
        {
            map[0] = ""abc"";
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = (ElementAccessExpressionSyntax)syntaxTree.FindAssignmentExpression("map[0] = \"abc\"").Left;
                var expected = (IPropertySymbol)semanticModel.GetTypeInfo(node.Expression).Type.GetMembers().Single(x => x is IPropertySymbol property && property.IsIndexer);
                Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true,     semanticModel.TryGetSymbol(node, CancellationToken.None, out var indexer));
                Assert.AreEqual(expected, indexer);
            }

            [Test]
            public void MethodDeclaration()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class C
    {
        internal void M()
        {
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.FindMethodDeclaration("M");
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
    internal class C
    {
        internal C(int i, double d)
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
    internal class C
    {
        internal C()
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

            [TestCase("out var i")]
            public void VariableDesignationOutVar(string designation)
            [Test]
            public void VariableDesignationOutVar()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    class C
    {
        bool P => int.TryParse(string.Empty, out var i);
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

            [Test]
            public void DeclarationExpressionSyntaxOutVar()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    class C
    {
        bool P => int.TryParse(string.Empty, out var i);
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.Find<DeclarationExpressionSyntax>("var i");
                var expected = semanticModel.GetDeclaredSymbol(node.Designation);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class C
    {
        internal C()
        {
            int.TryParse(string.Empty, out var i);
        }
    }
}".AssertReplace("out var i", designation));
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.Find<VariableDesignationSyntax>(designation);
                var expected = semanticModel.GetDeclaredSymbol(node);
                Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(expected, type);
                Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            }

            [Test]
            public void VariableDesignationOutDiscard()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(
                    @"
namespace RoslynSandbox
{
    internal class C
    {
        internal C()
        {
            int.TryParse(string.Empty, out _);
        }
    }
}");
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var otherModel = compilation.GetSemanticModel(OtherTree);
                var node = syntaxTree.Find<ArgumentSyntax>("out _").Expression;
                Assert.AreEqual(SymbolKind.Discard, semanticModel.GetSymbolSafe(node, CancellationToken.None).Kind);
                Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
                Assert.AreEqual(SymbolKind.Discard, type.Kind);
                Assert.AreEqual(SymbolKind.Discard, otherModel.GetSymbolSafe(node, CancellationToken.None).Kind);
            }
        }
    }
}

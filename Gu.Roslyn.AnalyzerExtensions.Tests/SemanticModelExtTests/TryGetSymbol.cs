namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class TryGetSymbol
    {
        private static readonly SyntaxTree OtherTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        private readonly int bar;
    }
}");

        [Test]
        public static void ClassDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
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
        public static void FieldDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
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
        public static void ConstructorDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        C(int i, double d)
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
        public static void EventDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    class C
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
        public static void EventFieldDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    class C
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
        public static void PropertyDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
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
        public static void IndexerDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
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
        public static void IndexerWhenArrayElementAccess()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
        public static void IndexerWhenMultidimensionalArrayElementAccess()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(expected, symbol);
        }

        [Test]
        public static void IndexerWhenListElementAccess()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
            var expected = (IPropertySymbol)semanticModel.GetTypeInfo(node.Expression).Type.GetMembers().Single(x => x is IPropertySymbol { IsIndexer: true });
            Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var indexer));
            Assert.AreEqual(expected, indexer);
        }

        [Test]
        public static void IndexerWhenDictionaryElementAccess()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
            var expected = (IPropertySymbol)semanticModel.GetTypeInfo(node.Expression).Type.GetMembers().Single(x => x is IPropertySymbol { IsIndexer: true });
            Assert.AreEqual(expected, semanticModel.GetSymbolSafe(node, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var indexer));
            Assert.AreEqual(expected, indexer);
        }

        [Test]
        public static void MethodDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M()
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
        public static void Parameter()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        C(int i, double d)
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
        public static void VariableDeclaration()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        C()
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
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(expected, symbol);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol local));
            Assert.AreEqual(expected, local);
            Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
        }

        [Test]
        public static void VariableDeclarator()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        C()
        {
            var i = 1;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<VariableDeclaratorSyntax>("i = 1");
            var expected = semanticModel.GetDeclaredSymbol(node);
            Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(expected, symbol);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol local));
            Assert.AreEqual(expected, local);
            Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
        }

        [Test]
        public static void VariableDesignationOutVar()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(expected, symbol);
            Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
        }

        [Test]
        public static void DeclarationExpressionSyntaxOutVar()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
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
            Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe((SyntaxNode)node, CancellationToken.None));
            Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe((ExpressionSyntax)node, CancellationToken.None));

            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(expected, symbol);
            Assert.AreEqual(true, semanticModel.TryGetSymbol((SyntaxNode)node, CancellationToken.None, out symbol));
            Assert.AreEqual(expected, symbol);
            Assert.AreEqual(true, semanticModel.TryGetSymbol((ExpressionSyntax)node, CancellationToken.None, out symbol));
            Assert.AreEqual(expected, symbol);

            Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
        }

        [Test]
        public static void DeclarationExpressionSyntaxOutDiscard()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        bool P => int.TryParse(string.Empty, out var _);
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);

            var node = syntaxTree.Find<DeclarationExpressionSyntax>("var _");
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe((SyntaxNode)node, CancellationToken.None).Kind);
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe((ExpressionSyntax)node, CancellationToken.None).Kind);

            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var symbol));
            Assert.AreEqual(SymbolKind.Discard, symbol.Kind);

            Assert.AreEqual(true, semanticModel.TryGetSymbol((SyntaxNode)node, CancellationToken.None, out symbol));
            Assert.AreEqual(SymbolKind.Discard, symbol.Kind);

            Assert.AreEqual(true, semanticModel.TryGetSymbol((ExpressionSyntax)node, CancellationToken.None, out symbol));
            Assert.AreEqual(SymbolKind.Discard, symbol.Kind);

            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe((SyntaxNode)node, CancellationToken.None).Kind);
            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe((ExpressionSyntax)node, CancellationToken.None).Kind);
        }

        [Test]
        public static void DeclarationPatternSyntax()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M(object o)
        {
            if (o is int i)
            {
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<DeclarationPatternSyntax>("int i");
            var expected = semanticModel.GetDeclaredSymbol(node.Designation);
            Assert.AreEqual(expected, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(expected, type);
            Assert.AreEqual(expected, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None));
        }

        [Test]
        public static void DeclarationPatternSyntaxIsPattern()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M(object o)
        {
            if (o is int x)
            {
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<DeclarationPatternSyntax>("int x");
            Assert.AreEqual(SymbolKind.Local, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(SymbolKind.Local, type.Kind);
            Assert.AreEqual(SymbolKind.Local, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
        }

        [Test]
        public static void DeclarationPatternSyntaxSwitchLabel()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M(object o)
        {
            switch (o)
            {
                case int x:
                    break;
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<DeclarationPatternSyntax>("int x");
            Assert.AreEqual(SymbolKind.Local, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(SymbolKind.Local, type.Kind);
            Assert.AreEqual(SymbolKind.Local, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
        }

        [Test]
        public static void DeclarationPatternSyntaxDiscardSwitchLabel()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M(object o)
        {
            switch (o)
            {
                case int _:
                    break;
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<DeclarationPatternSyntax>("int _");
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(SymbolKind.Discard, type.Kind);
            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
        }

        [Test]
        public static void DeclarationPatternSyntaxDiscardIsPattern()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        void M(object o)
        {
            if (o is int _)
            {
            }
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<DeclarationPatternSyntax>("int _");
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(SymbolKind.Discard, type.Kind);
            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
        }

        [Test]
        public static void DiscardDesignationSyntax()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        bool P => int.TryParse(string.Empty, out var _);
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);

            var node = syntaxTree.Find<DiscardDesignationSyntax>("var _");
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetDeclaredSymbolSafe((SyntaxNode)node, CancellationToken.None).Kind);

            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out var type));
            Assert.AreEqual(SymbolKind.Discard, type.Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol((SyntaxNode)node, CancellationToken.None, out type));
            Assert.AreEqual(SymbolKind.Discard, type.Kind);

            Assert.AreEqual(SymbolKind.Discard, otherModel.GetDeclaredSymbolSafe(node, CancellationToken.None).Kind);
        }

        [Test]
        public static void VariableOutDiscard()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        bool P => int.TryParse(string.Empty, out _);
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var otherModel = compilation.GetSemanticModel(OtherTree);
            var node = syntaxTree.Find<ArgumentSyntax>("out _").Expression;
            var symbol = semanticModel.GetSymbolInfo(node, CancellationToken.None).Symbol;
            Assert.AreEqual(SymbolKind.Discard, symbol.Kind);
            Assert.AreEqual(SymbolKind.Discard, semanticModel.GetSymbolSafe(node, CancellationToken.None).Kind);
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out symbol));
            Assert.AreEqual(SymbolKind.Discard, symbol.Kind);
            Assert.AreEqual(SymbolKind.Discard, otherModel.GetSymbolSafe(node, CancellationToken.None).Kind);
        }
    }
}

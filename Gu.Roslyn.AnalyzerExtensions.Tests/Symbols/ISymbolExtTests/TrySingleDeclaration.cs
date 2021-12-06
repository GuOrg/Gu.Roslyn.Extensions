namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.ISymbolExtTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class TrySingleDeclaration
    {
        [TestCase("value1")]
        [TestCase("value2")]
        public static void Field(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        private int value1;
        private int value2;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindFieldDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }

        [TestCase("Value1")]
        [TestCase("Value2")]
        public static void Property(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindPropertyDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }

        [TestCase("Value1")]
        [TestCase("Value2")]
        public static void Method(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value1() => 1;

        public int Value2() => 2;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out MethodDeclarationSyntax match));
            Assert.AreEqual(node, match);
        }

        [TestCase("Value1")]
        [TestCase("Value2")]
        public static void ExplicitMethod(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value1() => 1;

        public int Value2() => 2;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleMethodDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }

        [TestCase("Value1")]
        [TestCase("Value2")]
        public static void Accessor(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindPropertyDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).GetMethod;
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out AccessorDeclarationSyntax match));
            Assert.AreEqual(node.AccessorList.Accessors[0], match);
        }

        [TestCase("Value1")]
        [TestCase("Value2")]
        public static void ExplicitAccessor(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindPropertyDeclaration(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).GetMethod;
            Assert.AreEqual(true, symbol.TrySingleAccessorDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node.AccessorList.Accessors[0], match);
        }

        [TestCase("value1")]
        [TestCase("value2")]
        public static void Local(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            var value1 = 1;
            var value2 = 2;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<VariableDeclaratorSyntax>(name);
            var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }

        [Test]
        public static void LocalAsType()
        {
            var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            var value = 1;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<VariableDeclaratorSyntax>("value");
            var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out LocalDeclarationStatementSyntax statement));
            Assert.AreEqual("var value = 1;", statement.ToString());

            Assert.AreEqual(true,             symbol.TrySingleDeclaration(CancellationToken.None, out VariableDeclaratorSyntax declarator));
            Assert.AreEqual("value = 1", declarator.ToString());
        }

        [TestCase("value1")]
        [TestCase("value2")]
        public static void OutLocal(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            Out(out var value1);
            Out(out var value2);
        }

        private static void Out(out int value)
        {
            value = 1;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.Find<VariableDesignationSyntax>(name);
            var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }

        [TestCase("value1")]
        [TestCase("value2")]
        public static void Parameter(string name)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int value1, int value2)
        {
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindParameter(name);
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
            Assert.AreEqual(node, match);
        }
    }
}

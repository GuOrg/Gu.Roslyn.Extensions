namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public partial class ISymbolExtTests
    {
        public class TrySingleDeclaration
        {
            [TestCase("value1")]
            [TestCase("value2")]
            public void Field(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        private int value1;
        private int value2;
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindFieldDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }

            [TestCase("Value1")]
            [TestCase("Value2")]
            public void Property(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindPropertyDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }

            [TestCase("Value1")]
            [TestCase("Value2")]
            public void Method(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1() => 1;

        public int Value2() => 2;
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out MethodDeclarationSyntax match));
                Assert.AreEqual(node, match);
            }

            [TestCase("Value1")]
            [TestCase("Value2")]
            public void ExplicitMethod(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1() => 1;

        public int Value2() => 2;
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindMethodDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleMethodDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }

            [TestCase("Value1")]
            [TestCase("Value2")]
            public void Accessor(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindPropertyDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).GetMethod;
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out AccessorDeclarationSyntax match));
                Assert.AreEqual(node.AccessorList.Accessors[0], match);
            }

            [TestCase("Value1")]
            [TestCase("Value2")]
            public void ExplicitAccessor(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Value1 { get; }

        public int Value2 { get; }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindPropertyDeclaration(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None).GetMethod;
                Assert.AreEqual(true, symbol.TrySingleAccessorDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node.AccessorList.Accessors[0], match);
            }

            [TestCase("value1")]
            [TestCase("value2")]
            public void Local(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var value1 = 1;
            var value2 = 2;
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.Find<VariableDeclaratorSyntax>(name);
                var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }

            [TestCase("value1")]
            [TestCase("value2")]
            public void OutLocal(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
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
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.Find<VariableDesignationSyntax>(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }

            [TestCase("value1")]
            [TestCase("value2")]
            public void Parameter(string name)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int value1, int value2)
        {
        }
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var node = syntaxTree.FindParameter(name);
                var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
                Assert.AreEqual(true, symbol.TrySingleDeclaration(CancellationToken.None, out var match));
                Assert.AreEqual(node, match);
            }
        }
    }
}

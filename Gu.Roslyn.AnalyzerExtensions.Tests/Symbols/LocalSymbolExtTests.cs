namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class LocalSymbolExtTests
    {
        [Test]
        public void LocalInCtor()
        {
            var testCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public Foo()
        {
            var i = 0;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            Assert.AreEqual("public Foo()\r\n        {\r\n            var i = 0;\r\n        }", scope.ToString());
        }

        [Test]
        public void LocalInMethod()
        {
            var testCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public void Bar()
        {
            var i = 0;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            Assert.AreEqual("public void Bar()\r\n        {\r\n            var i = 0;\r\n        }", scope.ToString());
        }

        [Test]
        public void LocalInLocalFunction()
        {
            var testCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public void Bar()
        {
            void BarCore()
            {
                var i = 0;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            Assert.AreEqual("void BarCore()\r\n            {\r\n                var i = 0;\r\n            }", scope.ToString());
        }

        [Test]
        public void LocalInLambda()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    class Foo
    {
        public void Bar()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                var i = 0;
            };
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            var symbol = semanticModel.GetDeclaredSymbolSafe(node, CancellationToken.None);
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            Assert.AreEqual("(sender, args) =>\r\n            {\r\n                var i = 0;\r\n            }", scope.ToString());
        }
    }
}

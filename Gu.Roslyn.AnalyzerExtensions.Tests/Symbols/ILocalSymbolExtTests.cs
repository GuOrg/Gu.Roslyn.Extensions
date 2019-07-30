namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ILocalSymbolExtTests
    {
        [Test]
        public void LocalInCtor()
        {
            var code = @"
namespace N
{
    class C
    {
        public C()
        {
            var i = 0;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol symbol));
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            CodeAssert.AreEqual("public C()\r\n        {\r\n            var i = 0;\r\n        }", scope.ToString());
        }

        [Test]
        public void LocalInMethod()
        {
            var code = @"
namespace N
{
    class C
    {
        public void M()
        {
            var i = 0;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol symbol));
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            CodeAssert.AreEqual("public void M()\r\n        {\r\n            var i = 0;\r\n        }", scope.ToString());
        }

        [Test]
        public void LocalInLocalFunction()
        {
            var code = @"
namespace N
{
    class C
    {
        public void M()
        {
            void MCore()
            {
                var i = 0;
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol symbol));
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            CodeAssert.AreEqual("void MCore()\r\n            {\r\n                var i = 0;\r\n            }", scope.ToString());
        }

        [Test]
        public void LocalInLambda()
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
        public void M()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                var i = 0;
            };
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindVariableDeclaration("var i = 0");
            Assert.AreEqual(true, semanticModel.TryGetSymbol(node, CancellationToken.None, out ILocalSymbol symbol));
            Assert.AreEqual(true, symbol.TryGetScope(CancellationToken.None, out var scope));
            CodeAssert.AreEqual("(sender, args) =>\r\n            {\r\n                var i = 0;\r\n            }", scope.ToString());
        }
    }
}

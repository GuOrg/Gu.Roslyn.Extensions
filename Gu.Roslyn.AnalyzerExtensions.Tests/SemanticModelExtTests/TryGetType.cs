namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class TryGetType
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
            var node = syntaxTree.FindClassDeclaration("C");
            var expected = compilation.GetSemanticModel(syntaxTree).GetDeclaredSymbol(node);
            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetType(node, CancellationToken.None, out var type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, CancellationToken.None, out var namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetType(node, CancellationToken.None, out type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetNamedType(node, CancellationToken.None, out namedType));
            Assert.AreEqual(namedType, type);
        }

        [TestCase("[Obsolete]")]
        [TestCase("[ObsoleteAttribute]")]
        [TestCase("[System.Obsolete]")]
        [TestCase("[System.ObsoleteAttribute]")]
        public static void Attribute(string attribute)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    using System;

    [Obsolete]
    class C
    {
        private readonly int bar;
    }
}".AssertReplace("[Obsolete]", attribute));
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, MetadataReferences.FromAttributes());
            var node = syntaxTree.FindAttribute("Obsolete");
            var expected = compilation.GetSemanticModel(syntaxTree).GetTypeInfo(node).Type;
            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetType(node, CancellationToken.None, out var type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, CancellationToken.None, out var namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true,      compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, QualifiedType.System.ObsoleteAttribute, CancellationToken.None, out namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetType(node, CancellationToken.None, out type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetNamedType(node, CancellationToken.None, out namedType));
            Assert.AreEqual(namedType, type);
        }
    }
}

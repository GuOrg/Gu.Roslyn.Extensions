namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System.Threading;

    using Gu.Roslyn.Asserts;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            Assert.AreEqual(expected, compilation.GetSemanticModel(syntaxTree).GetType(node, CancellationToken.None));

            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, CancellationToken.None, out var namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetType(node, CancellationToken.None, out type));
            Assert.AreEqual(expected, type);
            Assert.AreEqual(expected, compilation.GetSemanticModel(OtherTree).GetType(node, CancellationToken.None));

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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, Settings.Default.MetadataReferences);
            var node = syntaxTree.FindAttribute("Obsolete");
            var expected = compilation.GetSemanticModel(syntaxTree).GetTypeInfo(node).Type;
            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetType(node, CancellationToken.None, out var type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, CancellationToken.None, out var namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(syntaxTree).TryGetNamedType(node, QualifiedType.System.ObsoleteAttribute, CancellationToken.None, out namedType));
            Assert.AreEqual(namedType, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetType(node, CancellationToken.None, out type));
            Assert.AreEqual(expected, type);

            Assert.AreEqual(true, compilation.GetSemanticModel(OtherTree).TryGetNamedType(node, CancellationToken.None, out namedType));
            Assert.AreEqual(namedType, type);
        }

        [TestCase("ObsoleteAttribute")]
        [TestCase("System.ObsoleteAttribute")]
        public static void TryGetNamedTypeWhenAliasWithSameName(string typeName)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;
    using Window = System.Windows.Window;

    [ObsoleteAttribute]
    internal class C
    {
    }
}".AssertReplace("ObsoleteAttribute", typeName));
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var qualifiedType = QualifiedType.FromType(typeof(System.ObsoleteAttribute));
            var attribute = syntaxTree.Find<AttributeSyntax>(typeName);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(attribute, qualifiedType, CancellationToken.None, out var type));
            Assert.AreEqual("ObsoleteAttribute", type.Name);

            qualifiedType = QualifiedType.FromType(typeof(System.Attribute));
            Assert.AreEqual(false, semanticModel.TryGetNamedType(attribute, qualifiedType, CancellationToken.None, out _));
        }

        [Test]
        public static void Nullable()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
#nullable enable
namespace N
{
    class C
    {
        public string? P { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, Settings.Default.MetadataReferences);
            var node = syntaxTree.FindPropertyDeclaration("P");
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var propertySymbol = semanticModel.GetDeclaredSymbol(node)!;

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetType(node.Type, CancellationToken.None));
            Assert.AreEqual(NullableAnnotation.Annotated, compilation.GetSemanticModel(syntaxTree).GetType(node.Type, CancellationToken.None).NullableAnnotation);
            Assert.AreEqual(true, semanticModel.TryGetType(node.Type, CancellationToken.None, out var type));
            Assert.AreEqual(propertySymbol.Type, type);

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetNamedType(node.Type, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetNamedType(node.Type, CancellationToken.None, out var namedType));
            Assert.AreEqual(propertySymbol.Type, namedType);
        }

        [Test]
        public static void NotNull()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
#nullable enable
namespace N
{
    class C
    {
        public string P { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, Settings.Default.MetadataReferences);
            var node = syntaxTree.FindPropertyDeclaration("P");
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var propertySymbol = semanticModel.GetDeclaredSymbol(node)!;

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetType(node.Type, CancellationToken.None));
            Assert.AreEqual(NullableAnnotation.NotAnnotated, compilation.GetSemanticModel(syntaxTree).GetType(node.Type, CancellationToken.None).NullableAnnotation);
            Assert.AreEqual(true, semanticModel.TryGetType(node.Type, CancellationToken.None, out var type));
            Assert.AreEqual(propertySymbol.Type, type);

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetNamedType(node.Type, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetNamedType(node.Type, CancellationToken.None, out var namedType));
            Assert.AreEqual(propertySymbol.Type, namedType);
        }

        [Test]
        public static void NullableDisabled()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class C
    {
        public string P { get; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, OtherTree }, Settings.Default.MetadataReferences);
            var node = syntaxTree.FindPropertyDeclaration("P");
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var propertySymbol = semanticModel.GetDeclaredSymbol(node)!;

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetType(node.Type, CancellationToken.None));
            Assert.AreEqual(NullableAnnotation.None, compilation.GetSemanticModel(syntaxTree).GetType(node.Type, CancellationToken.None).NullableAnnotation);
            Assert.AreEqual(true, semanticModel.TryGetType(node.Type, CancellationToken.None, out var type));
            Assert.AreEqual(propertySymbol.Type, type);

            Assert.AreEqual(propertySymbol.Type, semanticModel.GetNamedType(node.Type, CancellationToken.None));
            Assert.AreEqual(true, semanticModel.TryGetNamedType(node.Type, CancellationToken.None, out var namedType));
            Assert.AreEqual(propertySymbol.Type, namedType);
        }
    }
}

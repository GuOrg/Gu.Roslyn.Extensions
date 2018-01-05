namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class TypeSymbolExt
    {
        [Test]
        public void TryFindField()
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, typeSymbol.TryFindField("bar", out var field));
            Assert.AreEqual("bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFieldRecursive("bar", out field));
            Assert.AreEqual("bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("bar", out var member));
            Assert.AreEqual("bar", member.Name);

            Assert.AreEqual(false, typeSymbol.TryFindField("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFieldRecursive("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMember("missing", out _));
        }

        [Test]
        public void TryFindEvent()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    using System;

    internal class Foo
    {
        public event EventHandler Bar;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, typeSymbol.TryFindEvent("Bar", out var field));
            Assert.AreEqual("Bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindEventRecursive("Bar", out field));
            Assert.AreEqual("Bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("Bar", out var member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMemberRecursive("Bar", out member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(false, typeSymbol.TryFindEvent("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindEvent("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMember("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMemberRecursive("missing", out _));
        }

        [Test]
        public void TryFindProperty()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public int Bar { get; set; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, typeSymbol.TryFindProperty("Bar", out var field));
            Assert.AreEqual("Bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindPropertyRecursive("Bar", out field));
            Assert.AreEqual("Bar", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("Bar", out var member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMemberRecursive("Bar", out member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(false, typeSymbol.TryFindProperty("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindProperty("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMember("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMemberRecursive("missing", out _));
        }
    }
}

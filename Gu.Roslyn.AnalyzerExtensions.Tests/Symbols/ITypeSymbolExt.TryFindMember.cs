namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public partial class ITypeSymbolExtTests
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
            Assert.AreEqual(true, typeSymbol.TryFindProperty("Bar", out var property));
            Assert.AreEqual("Bar", property.Name);

            Assert.AreEqual(true, typeSymbol.TryFindPropertyRecursive("Bar", out property));
            Assert.AreEqual("Bar", property.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("Bar", out var member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMemberRecursive("Bar", out member));
            Assert.AreEqual("Bar", member.Name);

            Assert.AreEqual(false, typeSymbol.TryFindProperty("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindProperty("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMember("missing", out _));
            Assert.AreEqual(false, typeSymbol.TryFindFirstMemberRecursive("missing", out _));
        }

        [Test]
        public void TryFindMethodWhenOverridden()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
        public override string ToString() => ""abc"";
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var type = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, type.TryFindFirstMethod("ToString", out var method));
            Assert.AreEqual("ToString", method.Name);

            Assert.AreEqual(true, type.TryFindFirstMethodRecursive("ToString", out method));
            Assert.AreEqual("ToString", method.Name);

            Assert.AreEqual(true, type.TryFindSingleMethod("ToString", out method));
            Assert.AreEqual("ToString", method.Name);

            Assert.AreEqual(true, type.TryFindSingleMethodRecursive("ToString", out method));
            Assert.AreEqual("ToString", method.Name);

            Assert.AreEqual(true, type.TryFindFirstMember("ToString", out var member));
            Assert.AreEqual("ToString", member.Name);

            Assert.AreEqual(true, type.TryFindFirstMemberRecursive("ToString", out member));
            Assert.AreEqual("ToString", member.Name);

            Assert.AreEqual(false, type.TryFindFirstMethod("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindSingleMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindSingleMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMember("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMemberRecursive("missing", out _));
        }

        [TestCase("ToString")]
        [TestCase("ReferenceEquals")]
        public void TryFindMethod(string name)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    public class Foo
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var type = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(false, type.TryFindFirstMethod(name, out var method));

            Assert.AreEqual(true, type.TryFindFirstMethodRecursive(name, out method));
            Assert.AreEqual(name, method.Name);

            Assert.AreEqual(false, type.TryFindSingleMethod(name, out method));

            Assert.AreEqual(true, type.TryFindSingleMethodRecursive(name, out method));
            Assert.AreEqual(name, method.Name);

            Assert.AreEqual(false, type.TryFindFirstMember(name, out var member));

            Assert.AreEqual(true, type.TryFindFirstMemberRecursive(name, out member));
            Assert.AreEqual(name, member.Name);

            Assert.AreEqual(false, type.TryFindFirstMethod("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindSingleMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindSingleMethodRecursive("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMember("missing", out _));
            Assert.AreEqual(false, type.TryFindFirstMemberRecursive("missing", out _));
        }
    }
}

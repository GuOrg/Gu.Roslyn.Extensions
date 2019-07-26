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
namespace N
{
    public class C
    {
        private readonly int bar;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("C");
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
namespace N
{
    using System;

    internal class C
    {
        public event EventHandler E;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("C");
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, typeSymbol.TryFindEvent("E", out var field));
            Assert.AreEqual("E", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindEventRecursive("E", out field));
            Assert.AreEqual("E", field.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("E", out var member));
            Assert.AreEqual("E", member.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMemberRecursive("E", out member));
            Assert.AreEqual("E", member.Name);

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
namespace N
{
    public class C
    {
        public int P { get; set; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("C");
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(true, typeSymbol.TryFindProperty("P", out var property));
            Assert.AreEqual("P", property.Name);

            Assert.AreEqual(true, typeSymbol.TryFindPropertyRecursive("P", out property));
            Assert.AreEqual("P", property.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMember("P", out var member));
            Assert.AreEqual("P", member.Name);

            Assert.AreEqual(true, typeSymbol.TryFindFirstMemberRecursive("P", out member));
            Assert.AreEqual("P", member.Name);

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
namespace N
{
    public class C
    {
        public override string ToString() => ""abc"";
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("C");
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
namespace N
{
    public class C
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeDeclaration = syntaxTree.FindClassDeclaration("C");
            var type = semanticModel.GetDeclaredSymbol(typeDeclaration);
            Assert.AreEqual(false, type.TryFindFirstMethod(name, out _));
            Assert.AreEqual(false, type.TryFindSingleMethod(name, out _));
            Assert.AreEqual(false, type.TryFindFirstMember(name, out _));

            Assert.AreEqual(true, type.TryFindFirstMethodRecursive(name, out var method));
            Assert.AreEqual(name, method.Name);

            Assert.AreEqual(true, type.TryFindSingleMethodRecursive(name, out method));
            Assert.AreEqual(name, method.Name);

            Assert.AreEqual(true, type.TryFindFirstMemberRecursive(name, out var member));
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

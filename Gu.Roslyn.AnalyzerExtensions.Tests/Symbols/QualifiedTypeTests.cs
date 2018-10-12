namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class QualifiedTypeTests
    {
        [Test]
        public void SymbolEquality()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal object Bar()
        {
        }
    }
}");
            var compilation =
                CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("Bar");
            var typeSymbol = semanticModel.GetDeclaredSymbol(node).ReturnType;
            Assert.AreEqual(true,  typeSymbol == new QualifiedType("System.Object"));
            Assert.AreEqual(true,  typeSymbol == QualifiedType.System.Object);
            Assert.AreEqual(false, typeSymbol == QualifiedType.System.String);
            Assert.AreEqual(false, typeSymbol != new QualifiedType("System.Object"));
            Assert.AreEqual(true,  typeSymbol.IsAssignableTo(QualifiedType.System.Object, compilation));
            Assert.AreEqual(true,  typeSymbol.IsSameType(QualifiedType.System.Object, compilation));
            Assert.AreEqual(false, typeSymbol.IsAssignableTo(QualifiedType.System.String, compilation));
            Assert.AreEqual(false, typeSymbol.IsSameType(QualifiedType.System.String, compilation));
        }

        [TestCase(typeof(object), "Object",        "String")]
        [TestCase(typeof(object), "System.Object", "System.String")]
        [TestCase(typeof(object), "object",        "string")]
        public void TypeSyntaxEquality(Type type, string typeText, string otherTypeText)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal object Bar() { }
        internal string Other() { }
    }
}".AssertReplace("object", typeText)
  .AssertReplace("string", otherTypeText));
            var typeSyntax = syntaxTree.FindMethodDeclaration("Bar").ReturnType;
            var qualifiedType = QualifiedType.FromType(type);
            Assert.AreEqual(true,  typeSyntax == qualifiedType);
            Assert.AreEqual(false, typeSyntax != qualifiedType);

            typeSyntax = syntaxTree.FindMethodDeclaration("Other").ReturnType;
            Assert.AreEqual(false, typeSyntax == qualifiedType);
            Assert.AreEqual(true,  typeSyntax != qualifiedType);
        }
    }
}

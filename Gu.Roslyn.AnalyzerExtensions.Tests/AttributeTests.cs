namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class AttributeTests
    {
        [TestCase("[System.Obsolete]")]
        [TestCase("[Obsolete]")]
        [TestCase("[ObsoleteAttribute]")]
        [TestCase("[System.ObsoleteAttribute]")]
        public void Test(string attribute)
        {
            string code = @"
namespace RoslynSandbox
{
    using System;

    [Obsolete]
    public class Foo
    {
    }
}";
            code = code.AssertReplace("[Obsolete]", attribute);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var classDeclaration = syntaxTree.FindClassDeclaration("Foo");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = new QualifiedType("System.ObsoleteAttribute");
            Assert.AreEqual(true, Attribute.TryFind(classDeclaration.AttributeLists, type, semanticModel, CancellationToken.None, out var match));
            Assert.AreEqual(attribute, $"[{match}]");
        }
    }
}

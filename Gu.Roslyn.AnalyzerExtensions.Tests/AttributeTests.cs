namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class AttributeTests
    {
        [TestCase("[System.Obsolete]")]
        [TestCase("[Obsolete]")]
        [TestCase("[ObsoleteAttribute]")]
        [TestCase("[System.ObsoleteAttribute]")]
        public static void Test(string attribute)
        {
            string code = @"
namespace N
{
    using System;

    [Obsolete]
    public class C
    {
    }
}".AssertReplace("[Obsolete]", attribute);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var classDeclaration = syntaxTree.FindClassDeclaration("C");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = new QualifiedType("System.ObsoleteAttribute");
            Assert.AreEqual(true, Attribute.TryFind(classDeclaration.AttributeLists, type, semanticModel, CancellationToken.None, out var match));
            Assert.AreEqual(attribute, $"[{match}]");
        }
    }
}

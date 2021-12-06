namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public static class IPropertySymbolExtTests
    {
        [TestCase("GetOnly",        true)]
        [TestCase("AutoGetSet",     true)]
        [TestCase("AbstractGetSet", true)]
        [TestCase("ExpressionBody", false)]
        public static void IsAutoProperty(string name, bool expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public abstract class C
    {
        public int GetOnly { get; }

        public int AutoGetSet { get; set; }

        public abstract int AbstractGetSet { get; set; }

        public int ExpressionBody => GetOnly;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration(name));
            Assert.AreEqual(expected, property.IsAutoProperty());
        }

        [TestCase("GetOnly", true)]
        [TestCase("AutoGetSet", false)]
        [TestCase("ExpressionBody", false)]
        public static void IsGetOnly(string name, bool expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
        public int GetOnly { get; }

        public int AutoGetSet { get; set; }

        public int ExpressionBody => GetOnly;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration(name));
            Assert.AreEqual(expected, property.IsGetOnly());
        }
    }
}

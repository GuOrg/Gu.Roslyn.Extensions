namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.ISymbolExtTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class IsEquivalentTo
    {
        [Test]
        public static void PropertyIsEquivalentTo()
        {
            var code = @"
namespace N
{
    public abstract class CBase<T>
    {
        protected CBase()
        {
            this.Value = default(T);
        }

        public abstract T Value { get; set; }
    }

    public class C : CBase<int>
    {
        public C()
        {
            var temp = this.Value;
        }

        public override int Value { get; set; }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindPropertyDeclaration("public override int Value { get; set; }"), CancellationToken.None);
            Assert.AreEqual(true, symbol.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OriginalDefinition.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OverriddenProperty.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OverriddenProperty.OriginalDefinition.IsEquivalentTo(symbol));
        }

        [Test]
        public static void ExtensionMethod()
        {
            var code = @"
namespace N
{
    public static class C
    {
        public static text M(this string text) => text;

        public static string Get => string.Empty.M();
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindMethodDeclaration("M"), CancellationToken.None, out var symbol));
            Assert.AreEqual(true, symbol.IsEquivalentTo(symbol));
            Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindInvocation("M()"), CancellationToken.None, out var reduced));
            Assert.AreEqual(true, symbol.IsEquivalentTo(reduced));
            Assert.AreEqual(true, reduced.IsEquivalentTo(symbol));
            Assert.AreEqual(true, reduced.IsEquivalentTo(reduced));
        }
    }
}

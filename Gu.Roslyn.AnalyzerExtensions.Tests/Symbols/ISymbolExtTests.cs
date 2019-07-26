namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public partial class ISymbolExtTests
    {
        [Test]
        public void PropertyIsEquivalentTo()
        {
            var testCode = @"
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
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindPropertyDeclaration("public override int Value { get; set; }"), CancellationToken.None);
            Assert.AreEqual(true, symbol.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OriginalDefinition.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OverriddenProperty.IsEquivalentTo(symbol));
            Assert.AreEqual(true, symbol.OverriddenProperty.OriginalDefinition.IsEquivalentTo(symbol));
        }
    }
}

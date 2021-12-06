namespace Gu.Roslyn.AnalyzerExtensions.Tests.MemberPathTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class TryFindLast
    {
        [TestCase("this.C", "C")]
        [TestCase("C", "C")]
        [TestCase("C.Inner", "Inner")]
        [TestCase("this.C.Inner", "Inner")]
        [TestCase("C.Inner.C", "C")]
        [TestCase("C.Inner.C.Inner", "Inner")]
        [TestCase("this.C.Inner.C.Inner", "Inner")]
        [TestCase("this.C?.Inner.C.Inner", "Inner")]
        [TestCase("this.C?.Inner?.C.Inner", "Inner")]
        [TestCase("this.C?.Inner?.C?.Inner", "Inner")]
        [TestCase("this.C.Inner?.C.Inner", "Inner")]
        [TestCase("this.C.Inner?.C?.Inner", "Inner")]
        [TestCase("this.C.Inner.C?.Inner", "Inner")]
        [TestCase("(meh as C)?.Inner", "Inner")]
        [TestCase("((C)meh)?.Inner", "Inner")]
        public static void PropertyOrField(string expression, string expected)
        {
            var code = @"
namespace N
{
    using System;

    public sealed class C
    {
        private readonly object meh;
        private readonly C C;

        public C Inner => this.C;

        public void M()
        {
            var temp = C.Inner;
        }
    }
}".AssertReplace("C.Inner", expression);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var value = syntaxTree.FindExpression(expression);
            Assert.AreEqual(true, MemberPath.TryFindLast(value, out var member));
            Assert.AreEqual(expected, member.ToString());

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
            Assert.AreEqual(expected, symbol.Name);
        }
    }
}

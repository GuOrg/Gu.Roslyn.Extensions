namespace Gu.Roslyn.AnalyzerExtensions.Tests.MemberPathTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class TryFindRoot
    {
        [TestCase("this.C", "C")]
        [TestCase("C", "C")]
        [TestCase("C.Inner", "C")]
        [TestCase("this.C.Inner", "C")]
        [TestCase("C.Inner.C", "C")]
        [TestCase("C.Inner.C.Inner", "C")]
        [TestCase("this.C.Inner.C.Inner", "C")]
        [TestCase("this.C?.Inner.C.Inner", "C")]
        [TestCase("this.C?.Inner?.C.Inner", "C")]
        [TestCase("this.C?.Inner?.C?.Inner", "C")]
        [TestCase("this.C.Inner?.C.Inner", "C")]
        [TestCase("this.C.Inner?.C?.Inner", "C")]
        [TestCase("this.C.Inner.C?.Inner", "C")]
        [TestCase("(meh as C)?.Inner", "meh")]
        [TestCase("((C)meh)?.Inner", "meh")]
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
            Assert.AreEqual(true, MemberPath.TryFindRoot(value, out var member));
            Assert.AreEqual(expected, member.ToString());

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
            Assert.AreEqual(expected, symbol.Name);
        }

        [TestCase("C.Get<IComparable>(1)", "C")]
        [TestCase("C.Get<System.IComparable>(1)", "C")]
        [TestCase("C.Get<int>(1)", "C")]
        [TestCase("this.C.Get<int>(1)", "C")]
        [TestCase("this.C.Inner.Get<int>(1)", "C")]
        [TestCase("this.C.Inner.C.Get<int>(1)", "C")]
        [TestCase("this.C?.Get<int>(1)", "C")]
        [TestCase("this.C?.C.Get<int>(1)", "C")]
        [TestCase("this.Inner?.Inner.Get<int>(1)", "Inner")]
        [TestCase("this.Inner?.C.Get<int>(1)", "Inner")]
        [TestCase("this.Inner?.C?.Get<int>(1)", "Inner")]
        [TestCase("this.Inner.C?.Get<int>(1)", "Inner")]
        [TestCase("this.Inner?.C?.Inner?.Get<int>(1)", "Inner")]
        [TestCase("((C)meh).Get<int>(1)", "meh")]
        [TestCase("((C)this.meh).Get<int>(1)", "meh")]
        [TestCase("((C)this.Inner.meh).Get<int>(1)", "Inner")]
        [TestCase("(meh as C).Get<int>(1)", "meh")]
        [TestCase("(this.meh as C).Get<int>(1)", "meh")]
        [TestCase("(this.Inner.meh as C).Get<int>(1)", "Inner")]
        [TestCase("(this.Inner.meh as C)?.Get<int>(1)", "Inner")]
        [TestCase("(meh as C)?.Get<int>(1)", "meh")]
        public static void ForMethodInvocation(string expression, string expected)
        {
            var code = @"
namespace N
{
    using System;

    public sealed class C : IDisposable
    {
        private readonly object meh;
        private readonly C C;

        public C Inner => this.C;

        public void Dispose()
        {
            var temp = this.C.Get<int>(1);
        }

        private T Get<T>(int value) => default(T);
    }
}";
            code = code.AssertReplace("this.C.Get<int>(1)", expression);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var invocation = syntaxTree.FindExpression(expression);
            Assert.AreEqual(true, MemberPath.TryFindRoot(invocation, out var member));
            Assert.AreEqual(expected, member.ToString());

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
            Assert.AreEqual(expected, symbol.Name);
        }

        [TestCase("C.Inner", "C")]
        public static void MemberAccessExpression(string expression, string expected)
        {
            var code = @"
namespace N
{
    public sealed class C
    {
        private readonly C C;

        public C(C C, C inner)
        {
            this.C = C;
            this.Inner = inner;
        }

        public static object M(C C) => C.Inner;

        public C Inner { get; }
    }
}";
            code = code.AssertReplace("C.Inner", code);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var value = syntaxTree.FindMemberAccessExpression(expression);
            Assert.AreEqual(true, MemberPath.TryFindRoot(value, out var member));
            Assert.AreEqual(expected, member.ToString());

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
            Assert.AreEqual(expected, symbol.Name);
        }

        [Test]
        public static void Recursive()
        {
            var code = @"
namespace N
{
    internal class C
    {
        private int value;

        public int Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var invocation = syntaxTree.FindMemberAccessExpression("this.Value");
            Assert.AreEqual(true, MemberPath.TryFindRoot(invocation, out var member));
            Assert.AreEqual("Value", member.ToString());
        }
    }
}

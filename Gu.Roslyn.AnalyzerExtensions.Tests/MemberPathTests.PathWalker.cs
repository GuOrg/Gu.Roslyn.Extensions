namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class MemberPathTests
    {
        public class PathWalker
        {
            [TestCase("C", "C")]
            [TestCase("C.Inner", "C.Inner")]
            [TestCase("C?.Inner", "C.Inner")]
            [TestCase("C.Inner.C", "C.Inner.C")]
            [TestCase("C.Inner.C.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C", "C")]
            [TestCase("this?.C", "C")]
            [TestCase("this.C.Inner", "C.Inner")]
            [TestCase("this.C?.Inner", "C.Inner")]
            [TestCase("this.C.Inner.C.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C?.Inner.C.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C?.Inner?.C.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C?.Inner?.C?.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C.Inner?.C.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C.Inner?.C?.Inner", "C.Inner.C.Inner")]
            [TestCase("this.C.Inner.C?.Inner", "C.Inner.C.Inner")]
            [TestCase("(meh as C)?.Inner", "meh.Inner")]
            [TestCase("((C)meh)?.Inner", "meh.Inner")]
            public void PropertyOrField(string code, string expected)
            {
                var testCode = @"
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
}";
                testCode = testCode.AssertReplace("C.Inner", code);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var value = syntaxTree.FindExpression(code);
                using (var walker = MemberPath.PathWalker.Borrow(value))
                {
                    Assert.AreEqual(expected, string.Join(".", walker.IdentifierNames.Select(x => x)));
                }
            }

            [TestCase("Equals(1, 1)", "")]
            [TestCase("ToString()", "")]
            [TestCase("this.ToString()", "")]
            [TestCase("C.Dispose()", "C")]
            [TestCase("C.ToString().ToString()", "C")]
            [TestCase("C?.Dispose()", "C")]
            [TestCase("Inner?.Dispose()", "Inner")]
            [TestCase("C.Get<IComparable>(1)", "C")]
            [TestCase("C?.Get<IComparable>(1)", "C")]
            [TestCase("C.Get<System.IComparable>(1)", "C")]
            [TestCase("C.Get<int>(1)", "C")]
            [TestCase("this.C.Get<int>(1)", "C")]
            [TestCase("this.C.Dispose()", "C")]
            [TestCase("this.C.ToString().ToString()", "C")]
            [TestCase("this.C?.Dispose()", "C")]
            [TestCase("this?.C?.Dispose()", "C")]
            [TestCase("this.C.Inner.Get<int>(1)", "C.Inner")]
            [TestCase("this.C.Inner.C.Get<int>(1)", "C.Inner.C")]
            [TestCase("this.C?.Get<int>(1)", "C")]
            [TestCase("this.C?.C.Get<int>(1)", "C.C")]
            [TestCase("this.Inner?.Inner.Get<int>(1)", "Inner.Inner")]
            [TestCase("this.Inner?.C.Get<int>(1)", "Inner.C")]
            [TestCase("this.Inner?.C?.Get<int>(1)", "Inner.C")]
            [TestCase("this.Inner.C?.Get<int>(1)", "Inner.C")]
            [TestCase("this.Inner?.C?.Inner?.Get<int>(1)", "Inner.C.Inner")]
            [TestCase("((C)meh).Get<int>(1)", "meh")]
            [TestCase("((C)this.meh).Get<int>(1)", "meh")]
            [TestCase("((C)this.Inner.meh).Get<int>(1)", "Inner.meh")]
            [TestCase("(meh as C).Get<int>(1)", "meh")]
            [TestCase("(this.meh as C).Get<int>(1)", "meh")]
            [TestCase("(this.Inner.meh as C).Get<int>(1)", "Inner.meh")]
            [TestCase("(this.Inner.meh as C)?.Get<int>(1)", "Inner.meh")]
            [TestCase("(meh as C)?.Get<int>(1)", "meh")]
            public void Invocation(string code, string expected)
            {
                var testCode = @"
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
                testCode = testCode.AssertReplace("this.C.Get<int>(1)", code);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                using (var walker = MemberPath.PathWalker.Borrow(syntaxTree.FindInvocation(code)))
                {
                    Assert.AreEqual(expected, string.Join(".", walker.IdentifierNames.Select(x => x)));
                }

                using (var walker = MemberPath.PathWalker.Borrow(syntaxTree.FindExpression(code)))
                {
                    Assert.AreEqual(expected, string.Join(".", walker.IdentifierNames.Select(x => x)));
                }
            }

            [Test]
            public void Recursive()
            {
                var testCode = @"
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
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var invocation = syntaxTree.FindMemberAccessExpression("this.Value");
                using (var walker = MemberPath.PathWalker.Borrow(invocation))
                {
                    Assert.AreEqual("Value", string.Join(".", walker.IdentifierNames.Select(x => x)));
                }
            }
        }
    }
}

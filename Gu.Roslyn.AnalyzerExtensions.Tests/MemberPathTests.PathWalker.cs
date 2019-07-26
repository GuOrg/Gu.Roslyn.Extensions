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
            [TestCase("foo", "foo")]
            [TestCase("foo.Inner", "foo.Inner")]
            [TestCase("foo?.Inner", "foo.Inner")]
            [TestCase("foo.Inner.foo", "foo.Inner.foo")]
            [TestCase("foo.Inner.foo.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo", "foo")]
            [TestCase("this?.foo", "foo")]
            [TestCase("this.foo.Inner", "foo.Inner")]
            [TestCase("this.foo?.Inner", "foo.Inner")]
            [TestCase("this.foo.Inner.foo.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo?.Inner.foo.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo?.Inner?.foo.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo?.Inner?.foo?.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo.Inner?.foo.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo.Inner?.foo?.Inner", "foo.Inner.foo.Inner")]
            [TestCase("this.foo.Inner.foo?.Inner", "foo.Inner.foo.Inner")]
            [TestCase("(meh as Foo)?.Inner", "meh.Inner")]
            [TestCase("((Foo)meh)?.Inner", "meh.Inner")]
            public void PropertyOrField(string code, string expected)
            {
                var testCode = @"
namespace N
{
    using System;

    public sealed class Foo
    {
        private readonly object meh;
        private readonly Foo foo;

        public Foo Inner => this.foo;

        public void Bar()
        {
            var temp = foo.Inner;
        }
    }
}";
                testCode = testCode.AssertReplace("foo.Inner", code);
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
            [TestCase("foo.Dispose()", "foo")]
            [TestCase("foo.ToString().ToString()", "foo")]
            [TestCase("foo?.Dispose()", "foo")]
            [TestCase("Inner?.Dispose()", "Inner")]
            [TestCase("foo.Get<IComparable>(1)", "foo")]
            [TestCase("foo?.Get<IComparable>(1)", "foo")]
            [TestCase("foo.Get<System.IComparable>(1)", "foo")]
            [TestCase("foo.Get<int>(1)", "foo")]
            [TestCase("this.foo.Get<int>(1)", "foo")]
            [TestCase("this.foo.Dispose()", "foo")]
            [TestCase("this.foo.ToString().ToString()", "foo")]
            [TestCase("this.foo?.Dispose()", "foo")]
            [TestCase("this?.foo?.Dispose()", "foo")]
            [TestCase("this.foo.Inner.Get<int>(1)", "foo.Inner")]
            [TestCase("this.foo.Inner.foo.Get<int>(1)", "foo.Inner.foo")]
            [TestCase("this.foo?.Get<int>(1)", "foo")]
            [TestCase("this.foo?.foo.Get<int>(1)", "foo.foo")]
            [TestCase("this.Inner?.Inner.Get<int>(1)", "Inner.Inner")]
            [TestCase("this.Inner?.foo.Get<int>(1)", "Inner.foo")]
            [TestCase("this.Inner?.foo?.Get<int>(1)", "Inner.foo")]
            [TestCase("this.Inner.foo?.Get<int>(1)", "Inner.foo")]
            [TestCase("this.Inner?.foo?.Inner?.Get<int>(1)", "Inner.foo.Inner")]
            [TestCase("((Foo)meh).Get<int>(1)", "meh")]
            [TestCase("((Foo)this.meh).Get<int>(1)", "meh")]
            [TestCase("((Foo)this.Inner.meh).Get<int>(1)", "Inner.meh")]
            [TestCase("(meh as Foo).Get<int>(1)", "meh")]
            [TestCase("(this.meh as Foo).Get<int>(1)", "meh")]
            [TestCase("(this.Inner.meh as Foo).Get<int>(1)", "Inner.meh")]
            [TestCase("(this.Inner.meh as Foo)?.Get<int>(1)", "Inner.meh")]
            [TestCase("(meh as Foo)?.Get<int>(1)", "meh")]
            public void Invocation(string code, string expected)
            {
                var testCode = @"
namespace N
{
    using System;

    public sealed class Foo : IDisposable
    {
        private readonly object meh;
        private readonly Foo foo;

        public Foo Inner => this.foo;

        public void Dispose()
        {
            var temp = this.foo.Get<int>(1);
        }

        private T Get<T>(int value) => default(T);
    }
}";
                testCode = testCode.AssertReplace("this.foo.Get<int>(1)", code);
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
    internal class Foo
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

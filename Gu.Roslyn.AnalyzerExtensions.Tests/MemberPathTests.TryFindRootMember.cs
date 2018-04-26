namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    internal partial class MemberPathTests
    {
        internal class TryFindRootMember
        {
            [TestCase("this.foo", "foo")]
            [TestCase("foo", "foo")]
            [TestCase("foo.Inner", "foo")]
            [TestCase("this.foo.Inner", "foo")]
            [TestCase("foo.Inner.foo", "foo")]
            [TestCase("foo.Inner.foo.Inner", "foo")]
            [TestCase("this.foo.Inner.foo.Inner", "foo")]
            [TestCase("this.foo?.Inner.foo.Inner", "foo")]
            [TestCase("this.foo?.Inner?.foo.Inner", "foo")]
            [TestCase("this.foo?.Inner?.foo?.Inner", "foo")]
            [TestCase("this.foo.Inner?.foo.Inner", "foo")]
            [TestCase("this.foo.Inner?.foo?.Inner", "foo")]
            [TestCase("this.foo.Inner.foo?.Inner", "foo")]
            [TestCase("(meh as Foo)?.Inner", "meh")]
            [TestCase("((Foo)meh)?.Inner", "meh")]
            public void PropertyOrField(string code, string expected)
            {
                var testCode = @"
namespace RoslynSandbox
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
                Assert.AreEqual(true, MemberPath.TryFindRoot(value, out var member));
                Assert.AreEqual(expected, member.ToString());

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
                Assert.AreEqual(expected, symbol.Name);
            }

            [TestCase("foo.Get<IComparable>(1)", "foo")]
            [TestCase("foo.Get<System.IComparable>(1)", "foo")]
            [TestCase("foo.Get<int>(1)", "foo")]
            [TestCase("this.foo.Get<int>(1)", "foo")]
            [TestCase("this.foo.Inner.Get<int>(1)", "foo")]
            [TestCase("this.foo.Inner.foo.Get<int>(1)", "foo")]
            [TestCase("this.foo?.Get<int>(1)", "foo")]
            [TestCase("this.foo?.foo.Get<int>(1)", "foo")]
            [TestCase("this.Inner?.Inner.Get<int>(1)", "Inner")]
            [TestCase("this.Inner?.foo.Get<int>(1)", "Inner")]
            [TestCase("this.Inner?.foo?.Get<int>(1)", "Inner")]
            [TestCase("this.Inner.foo?.Get<int>(1)", "Inner")]
            [TestCase("this.Inner?.foo?.Inner?.Get<int>(1)", "Inner")]
            [TestCase("((Foo)meh).Get<int>(1)", "meh")]
            [TestCase("((Foo)this.meh).Get<int>(1)", "meh")]
            [TestCase("((Foo)this.Inner.meh).Get<int>(1)", "Inner")]
            [TestCase("(meh as Foo).Get<int>(1)", "meh")]
            [TestCase("(this.meh as Foo).Get<int>(1)", "meh")]
            [TestCase("(this.Inner.meh as Foo).Get<int>(1)", "Inner")]
            [TestCase("(this.Inner.meh as Foo)?.Get<int>(1)", "Inner")]
            [TestCase("(meh as Foo)?.Get<int>(1)", "meh")]
            public void ForMethodInvocation(string code, string expected)
            {
                var testCode = @"
namespace RoslynSandbox
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
                var invocation = syntaxTree.FindExpression(code);
                Assert.AreEqual(true, MemberPath.TryFindRoot(invocation, out var member));
                Assert.AreEqual(expected, member.ToString());

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
                Assert.AreEqual(expected, symbol.Name);
            }

            [TestCase("foo.Inner", "foo")]
            public void MemberAccessExpression(string code, string expected)
            {
                var testCode = @"
namespace RoslynSandbox
{
    public sealed class Foo
    {
        private readonly Foo foo;

        public Foo(Foo foo, Foo inner)
        {
            this.foo = foo;
            this.Inner = inner;
        }

        public static object Bar(Foo foo) => foo.Inner;

        public Foo Inner { get; }
    }
}";
                testCode = testCode.AssertReplace("foo.Inner", code);
                var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
                var value = syntaxTree.FindMemberAccessExpression(code);
                Assert.AreEqual(true, MemberPath.TryFindRoot(value, out var member));
                Assert.AreEqual(expected, member.ToString());

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var symbol = semanticModel.GetSymbolSafe(member, CancellationToken.None);
                Assert.AreEqual(expected, symbol.Name);
            }

            [Test]
            public void Recursive()
            {
                var testCode = @"
namespace RoslynSandbox
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
                Assert.AreEqual(true, MemberPath.TryFindRoot(invocation, out var member));
                Assert.AreEqual("Value", member.ToString());
            }
        }
    }
}

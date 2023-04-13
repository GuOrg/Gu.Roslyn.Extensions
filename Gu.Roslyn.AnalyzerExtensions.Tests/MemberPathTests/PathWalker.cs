namespace Gu.Roslyn.AnalyzerExtensions.Tests.MemberPathTests;

using System.Linq;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class PathWalker
{
    [TestCase("f", "f")]
    [TestCase("f.P", "f.P")]
    [TestCase("f?.P", "f.P")]
    [TestCase("f.P.f", "f.P.f")]
    [TestCase("f.P.f.P", "f.P.f.P")]
    [TestCase("this.f", "f")]
    [TestCase("this?.f", "f")]
    [TestCase("this.f.P", "f.P")]
    [TestCase("this.f?.P", "f.P")]
    [TestCase("this.f.P.f.P", "f.P.f.P")]
    [TestCase("this.f?.P.f.P", "f.P.f.P")]
    [TestCase("this.f?.P?.f.P", "f.P.f.P")]
    [TestCase("this.f?.P?.f?.P", "f.P.f.P")]
    [TestCase("this.f.P?.f.P", "f.P.f.P")]
    [TestCase("this.f.P?.f?.P", "f.P.f.P")]
    [TestCase("this.f.P.f?.P", "f.P.f.P")]
    [TestCase("(meh as C)?.P", "meh.P")]
    [TestCase("((C)meh)?.P", "meh.P")]
    public static void PropertyOrField(string expression, string expected)
    {
        var code = @"
namespace N
{
    using System;

    public sealed class C
    {
        private readonly object meh;
        private readonly C f;

        public C P => this.f;

        public void M()
        {
            var temp = f.P;
        }
    }
}".AssertReplace("f.P", expression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var value = syntaxTree.FindExpression(expression);
        using var walker = MemberPath.PathWalker.Borrow(value);
        Assert.AreEqual(expected, string.Join(".", walker.Tokens.Select(x => x)));
    }

    [TestCase("Equals(1, 1)", "")]
    [TestCase("Object.Equals(1, 1)", "Object")]
    [TestCase("object.Equals(1, 1)", "object")]
    [TestCase("ToString()", "")]
    [TestCase("this.ToString()", "")]
    [TestCase("f.Dispose()", "f")]
    [TestCase("f.ToString().ToString()", "f")]
    [TestCase("f?.Dispose()", "f")]
    [TestCase("P?.Dispose()", "P")]
    [TestCase("f.Get<IComparable>(1)", "f")]
    [TestCase("f?.Get<IComparable>(1)", "f")]
    [TestCase("f.Get<System.IComparable>(1)", "f")]
    [TestCase("f.Get<int>(1)", "f")]
    [TestCase("this.f.Get<int>(1)", "f")]
    [TestCase("this.f.Dispose()", "f")]
    [TestCase("this.f.ToString().ToString()", "f")]
    [TestCase("this.f?.Dispose()", "f")]
    [TestCase("this?.f?.Dispose()", "f")]
    [TestCase("this.f.P.Get<int>(1)", "f.P")]
    [TestCase("this.f.P.f.Get<int>(1)", "f.P.f")]
    [TestCase("this.f?.Get<int>(1)", "f")]
    [TestCase("this.f?.f.Get<int>(1)", "f.f")]
    [TestCase("this.P?.P.Get<int>(1)", "P.P")]
    [TestCase("this.P?.f.Get<int>(1)", "P.f")]
    [TestCase("this.P?.f?.Get<int>(1)", "P.f")]
    [TestCase("this.P.f?.Get<int>(1)", "P.f")]
    [TestCase("this.P?.f?.P?.Get<int>(1)", "P.f.P")]
    [TestCase("((C)meh).Get<int>(1)", "meh")]
    [TestCase("((C)this.meh).Get<int>(1)", "meh")]
    [TestCase("((C)this.P.meh).Get<int>(1)", "P.meh")]
    [TestCase("(meh as C).Get<int>(1)", "meh")]
    [TestCase("(this.meh as C).Get<int>(1)", "meh")]
    [TestCase("(this.P.meh as C).Get<int>(1)", "P.meh")]
    [TestCase("(this.P.meh as C)?.Get<int>(1)", "P.meh")]
    [TestCase("(meh as C)?.Get<int>(1)", "meh")]
    public static void Invocation(string expression, string expected)
    {
        var code = @"
namespace N
{
    using System;

    public sealed class C : IDisposable
    {
        private readonly object meh;
        private readonly C f;

        public C P => this.f;

        public void Dispose()
        {
            var temp = this.f.Get<int>(1);
        }

        private T Get<T>(int value) => default(T);
    }
}".AssertReplace("this.f.Get<int>(1)", expression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        using (var walker = MemberPath.PathWalker.Borrow(syntaxTree.FindInvocation(expression)))
        {
            Assert.AreEqual(expected, string.Join(".", walker.Tokens.Select(x => x)));
        }

        using (var walker = MemberPath.PathWalker.Borrow(syntaxTree.FindExpression(expression)))
        {
            Assert.AreEqual(expected, string.Join(".", walker.Tokens.Select(x => x)));
        }
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
        using var walker = MemberPath.PathWalker.Borrow(invocation);
        Assert.AreEqual("Value", string.Join(".", walker.Tokens.Select(x => x)));
    }

    [TestCase("this.f!.P", "f.P")]
    [TestCase("this.f!.P!", "f.P")]
    [TestCase("this.f!.P!.f",  "f.P.f")]
    [TestCase("this.f!.P!.f!", "f.P.f")]
    public static void Bang(string expression, string expected)
    {
        var code = @"
namespace N
{
    using System;

    public sealed class C
    {
        private readonly C? f;

        public C? P => this.f;

        public void M()
        {
            var temp = this.f!.P;
        }
    }
}".AssertReplace("this.f!.P", expression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var value = syntaxTree.FindExpression(expression);
        using var walker = MemberPath.PathWalker.Borrow(value);
        Assert.AreEqual(expected, string.Join(".", walker.Tokens.Select(x => x)));
    }
}

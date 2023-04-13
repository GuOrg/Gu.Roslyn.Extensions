namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests;

using System.Linq;
using System.Threading.Tasks;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;

public static class Seal
{
    [TestCase("class C",        "sealed class C")]
    [TestCase("public class C", "public sealed class C")]
    public static async Task EmptyClass(string before, string after)
    {
        var code = @"
namespace N
{
    class C
    {
    }
}".AssertReplace("class C", before);
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

        var expected = @"
namespace N
{
    sealed class C
    {
    }
}".AssertReplace("sealed class C", after);
        _ = editor.Seal(editor.OriginalRoot.Find<ClassDeclarationSyntax>("class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [TestCase("partial class C", "sealed partial class C")]
    [TestCase("public partial class C", "public sealed partial class C")]
    public static async Task PartialClass(string before, string after)
    {
        var code = @"
namespace N
{
    partial class C
    {
    }
}".AssertReplace("partial class C", before);
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

        var expected = @"
namespace N
{
    sealed partial class C
    {
    }
}".AssertReplace("sealed partial class C", after);
        _ = editor.Seal(editor.OriginalRoot.Find<ClassDeclarationSyntax>("class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [TestCase("protected int f;",                      "private int f;")]
    [TestCase("protected int P { get; }",              "private int P { get; }")]
    [TestCase("protected virtual int P { get; }",      "private int P { get; }")]
    [TestCase("protected int P { get; private set; }", "private int P { get; set; }")]
    [TestCase("protected void M { }",                  "private void M { }")]
    [TestCase("protected virtual void M { }",          "private void M { }")]
    public static async Task WithProtectedMember(string before, string after)
    {
        var code = @"
namespace N
{
    class C
    {
        protected int f;
    }
}".AssertReplace("protected int f;", before);
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

        var expected = @"
namespace N
{
    sealed class C
    {
        private int f;
    }
}".AssertReplace("private int f;", after);
        _ = editor.Seal(editor.OriginalRoot.Find<ClassDeclarationSyntax>("class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task WithProtectedOverride()
    {
        var code = @"
namespace N
{
    public class BaseClass
    {
        public virtual int P1 { get; protected set }

        protected virtual int P2 { get; set }

        protected virtual void M() { }
    }

    public class C : BaseClass
    {
        public override int P1 { get; protected set; }

        protected override int P2 { get; set }

        protected override void M() { }
    }
}";
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

        var expected = @"
namespace N
{
    public class BaseClass
    {
        public virtual int P1 { get; protected set }

        protected virtual int P2 { get; set }

        protected virtual void M() { }
    }

    public sealed class C : BaseClass
    {
        public override int P1 { get; protected set; }

        protected override int P2 { get; set }

        protected override void M() { }
    }
}";
        _ = editor.Seal(editor.OriginalRoot.Find<ClassDeclarationSyntax>("class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task WithNestedProtected()
    {
        var code = @"
namespace N
{
    public class C
    {
        private class C1
        {
            protected int F;

            protected int P { get; }

            protected void M() { }
        }
    }
}";
        var sln = CodeFactory.CreateSolution(code);
        var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

        var expected = @"
namespace N
{
    public sealed class C
    {
        private class C1
        {
            protected int F;

            protected int P { get; }

            protected void M() { }
        }
    }
}";
        _ = editor.Seal(editor.OriginalRoot.Find<ClassDeclarationSyntax>("public class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }
}

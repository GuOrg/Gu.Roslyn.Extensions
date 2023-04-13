namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests;

using System.Linq;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;

public static class SortMembers
{
    [Test]
    public static void EmptyClass()
    {
        var code = @"
namespace N
{
    class C
    {
    }
}";
        var editor = CreateDocumentEditor(code);

        var expected = @"
namespace N
{
    class C
    {
    }
}";
        _ = editor.SortMembers(editor.OriginalRoot.Find<ClassDeclarationSyntax>("class C"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static void ClassWithMembers()
    {
        var code = @"
namespace N
{
    using System;

    public class WithMembers
    {
        private const int f1 = 0;
        private const int f2 = 0;
        const int F3 = 0;
        const int F4 = 0;
        internal const int F5 = 0;
        internal const int F6 = 0;
        public const int F7 = 0;
        public const int F8 = 0;
        private static readonly int F9 = 0;
        private static readonly int F10 = 0;
        internal static readonly int F11 = 0;
        internal static readonly int F12 = 0;
        public static readonly int F13 = 0;
        public static readonly int F14 = 0;
        private int F15 = 0;
        private readonly int F16 = 0;
        protected int F17 = 0;
        protected readonly int F18 = 0;
        public int F19 = 0;
        public readonly int F20 = 0;

        private event EventHandler E1;

        public event EventHandler E2;

        private WithMembers() { }

        public WithMembers(int _) { }

        private enum E3 { }

        public enum E4 { }

        private struct S1 { }

        public struct S2 { }

        private class C1 { }

        class C2 { }

        private static class C3 { }

        public class C4 { }

        public static class C5 { }

        private static int P1 { get; }

        private static int P2 { get; set; }

        private int P3 { get; }

        private int P4 { get; set; }

        internal static int P5 { get; }

        internal static int P6 { get; set; }

        internal int P7 { get; }

        internal int P8 { get; set; }

        protected static int P9 { get; }

        protected static int P10 { get; set; }

        protected int P11 { get; }

        protected int P12 { get; set; }

        public static int P13 { get; }

        public static int P14 { get; set; }

        public int P15 { get; }

        public int P16 { get; set; }
    }
}";
        var editor = CreateDocumentEditor(code);

        var expected = @"
namespace N
{
    using System;

    public class WithMembers
    {
        public const int F7 = 0;
        public const int F8 = 0;

        public static readonly int F13 = 0;
        public static readonly int F14 = 0;

        public readonly int F20 = 0;

        public int F19 = 0;

        internal const int F5 = 0;
        internal const int F6 = 0;

        internal static readonly int F11 = 0;
        internal static readonly int F12 = 0;

        protected readonly int F18 = 0;

        protected int F17 = 0;

        private const int f1 = 0;
        private const int f2 = 0;

        const int F3 = 0;
        const int F4 = 0;

        private static readonly int F9 = 0;
        private static readonly int F10 = 0;

        private readonly int F16 = 0;

        private int F15 = 0;

        public WithMembers(int _) { }

        private WithMembers() { }

        public event EventHandler E2;

        private event EventHandler E1;

        public enum E4 { }

        private enum E3 { }

        public static int P13 { get; }

        public static int P14 { get; set; }

        public int P15 { get; }

        public int P16 { get; set; }

        internal static int P5 { get; }

        internal static int P6 { get; set; }

        internal int P7 { get; }

        internal int P8 { get; set; }

        protected static int P9 { get; }

        protected static int P10 { get; set; }

        protected int P11 { get; }

        protected int P12 { get; set; }

        private static int P1 { get; }

        private static int P2 { get; set; }

        private int P3 { get; }

        private int P4 { get; set; }

        public struct S2 { }

        private struct S1 { }

        public static class C5 { }

        public class C4 { }

        private static class C3 { }

        private class C1 { }

        class C2 { }
    }
}";
        _ = editor.SortMembers(editor.OriginalRoot.Find<ClassDeclarationSyntax>("public class WithMembers"));
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static void InternalPropertyBeforePublic()
    {
        var code = @"
namespace N
{
    class C
    {
        internal int P1 { get; set; }

        public int P2 { get; set; }
    }
}";
        var editor = CreateDocumentEditor(code);
        _ = editor.MoveAfter(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P1"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"));

        var expected = @"
namespace N
{
    class C
    {
        public int P2 { get; set; }

        internal int P1 { get; set; }
    }
}";
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static void InternalPropertyBeforePublicWithComments()
    {
        var code = @"
namespace N
{
    class C
    {
        // P1
        internal int P1 { get; set; }

        // P2
        public int P2 { get; set; }
    }
}";
        var editor = CreateDocumentEditor(code);
        _ = editor.MoveAfter(editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P1"), editor.OriginalRoot.Find<PropertyDeclarationSyntax>("P2"));

        var expected = @"
namespace N
{
    class C
    {
        // P2
        public int P2 { get; set; }

        // P1
        internal int P1 { get; set; }
    }
}";
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    private static DocumentEditor CreateDocumentEditor(string code)
    {
        var sln = CodeFactory.CreateSolution(code);
        return DocumentEditor.CreateAsync(sln.Projects.Single().Documents.Single()).Result;
    }
}

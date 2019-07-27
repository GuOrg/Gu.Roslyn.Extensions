namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static partial class DocumentEditorExtTests
    {
        public static class AddProperty
        {
            [Test]
            public static async Task BeforePropertyWhenFirstMember()
            {
                var code = @"
namespace N
{
    public class C
    {
        internal int P { get; }
    }
}";
                var sln = CodeFactory.CreateSolution(code);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var property = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty => 1;");

                var expected = @"
namespace N
{
    public class C
    {
        public int NewProperty => 1;

        internal int P { get; }
    }
}";
                _ = editor.AddProperty(containingType, property);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public static async Task AfterProperty()
            {
                var code = @"
namespace N
{
    public class C
    {
        public int P1 { get; }

        public int P2 { get; set; }
    }
}";
                var sln = CodeFactory.CreateSolution(code);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var property = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty => 1;");

                var expected = @"
namespace N
{
    public class C
    {
        public int P1 { get; }

        public int NewProperty => 1;

        public int P2 { get; set; }
    }
}";
                _ = editor.AddProperty(containingType, property);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public static async Task AfterPropertyWhenLastMember()
            {
                var code = @"
namespace N
{
    public class C
    {
        public int P { get; }
    }
}";
                var sln = CodeFactory.CreateSolution(code);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var property = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty => 1;");

                var expected = @"
namespace N
{
    public class C
    {
        public int P { get; }

        public int NewProperty => 1;
    }
}";
                _ = editor.AddProperty(containingType, property);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public static async Task BeforePropertyInConditional()
            {
                var code = @"
namespace N
{
    public class C
    {
#if true
        internal int P { get; }
#endif
    }
}";
                var sln = CodeFactory.CreateSolution(code);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var property = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty => 1;");

                var expected = @"
namespace N
{
    public class C
    {
        public int NewProperty => 1;

#if true
        internal int P { get; }
#endif
    }
}";
                _ = editor.AddProperty(containingType, property);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public static async Task AfterPropertyInConditional()
            {
                var code = @"
namespace N
{
    public class C
    {
#if true
        public int P { get; }
#endif
    }
}";
                var sln = CodeFactory.CreateSolution(code);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var property = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty => 1;");

                var expected = @"
namespace N
{
    public class C
    {
#if true
        public int P { get; }
#endif

        public int NewProperty => 1;
    }
}";
                _ = editor.AddProperty(containingType, property);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public static async Task TypicalClass()
            {
                var testCode = @"
namespace N
{
    public abstract class C
    {
        public int F1 = 1;
        private int f2;

        public C()
        {
        }

        private C(int i)
        {
        }

        public int P1 { get; set; }

        public void M1()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var declaration = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration("public int NewProperty { get; set; }");
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                _ = editor.AddProperty(containingType, declaration);
                var expected = @"
namespace N
{
    public abstract class C
    {
        public int F1 = 1;
        private int f2;

        public C()
        {
        }

        private C(int i)
        {
        }

        public int P1 { get; set; }

        public int NewProperty { get; set; }

        public void M1()
        {
        }
    }
}";
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}

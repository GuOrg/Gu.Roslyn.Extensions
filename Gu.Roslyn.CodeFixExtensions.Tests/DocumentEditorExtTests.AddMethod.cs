namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using NUnit.Framework;

    public partial class DocumentEditorExtTests
    {
        public class AddMethod
        {
            [Test]
            public async Task Private()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("Foo");
                var method = SyntaxFactory.ParseCompilationUnit("private int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }

        private int NewMethod() => 1;
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task Public()
            {
                var testCode = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("Foo");
                var method = SyntaxFactory.ParseCompilationUnit("public int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace RoslynSandbox
{
    public abstract class Foo
    {
        public int Filed1 = 1;
        private int filed1;

        public Foo()
        {
        }

        private Foo(int i)
        {
        }

        public int Prop1 { get; set; }

        public void Bar1()
        {
        }

        public int NewMethod() => 1;

        internal void Bar2()
        {
        }

        protected void Bar3()
        {
        }

        private static void Bar4()
        {
        }

        private void Bar5()
        {
        }
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}

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
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var method = SyntaxFactory.ParseCompilationUnit("private int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
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
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
        {
        }
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var method = SyntaxFactory.ParseCompilationUnit("public int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace N
{
    public abstract class C
    {
        public int Filed1 = 1;
        private int filed1;

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

        public int NewMethod() => 1;

        internal void M2()
        {
        }

        protected void M3()
        {
        }

        private static void M4()
        {
        }

        private void M5()
        {
        }
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task AfterPropertyWithPragma()
            {
                var testCode = @"
namespace N
{
    public abstract class C
    {
#pragma warning disable INPC002 // Mutable public property should notify.
        public int P { get; set; }
#pragma warning restore INPC002 // Mutable public property should notify.
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var method = SyntaxFactory.ParseCompilationUnit("public int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace N
{
    public abstract class C
    {
#pragma warning disable INPC002 // Mutable public property should notify.
        public int P { get; set; }
#pragma warning restore INPC002 // Mutable public property should notify.

        public int NewMethod() => 1;
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task AfterPropertyInConditional()
            {
                var testCode = @"
namespace N
{
    public class C
    {
#if true
     public int P { get; }
#endif
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var method = SyntaxFactory.ParseCompilationUnit("public int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace N
{
    public class C
    {
#if true
        public int P { get; }
#endif

        public int NewMethod() => 1;
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task BeforeMethodInConditional()
            {
                var testCode = @"
namespace N
{
    public class C
    {
#if true
     private int M() => 1;
#endif
    }
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
                var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
                var method = SyntaxFactory.ParseCompilationUnit("public int NewMethod() => 1;")
                                          .Members
                                          .Single()
                                          .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithTrailingTrivia(SyntaxFactory.ElasticMarker)
                                          .WithAdditionalAnnotations(Formatter.Annotation);

                var expected = @"
namespace N
{
    public class C
    {
        public int NewMethod() => 1;

#if true
        private int M() => 1;
#endif
    }
}";
                _ = editor.AddMethod(containingType, (MethodDeclarationSyntax)method);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}

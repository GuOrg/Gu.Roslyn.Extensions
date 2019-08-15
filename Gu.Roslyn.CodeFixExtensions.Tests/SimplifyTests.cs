namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public static class SimplifyTests
    {
        [Test]
        public static async Task AddEventWhenNoUsing()
        {
            var code = @"
namespace N
{
    class C
    {
    }
}";
            var sln = CodeFactory.CreateSolution(code, MetadataReferences.FromAttributes());
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var eventDeclaration = (EventFieldDeclarationSyntax)editor.Generator.EventDeclaration("E", SyntaxFactory.ParseTypeName("System.EventHandler"), Accessibility.Public)
                                                                      .WithSimplifiedNames();
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
            _ = editor.AddEvent(containingType, eventDeclaration);
            var expected = @"
namespace N
{
    class C
    {
        public event System.EventHandler E;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }

        [Test]
        public static async Task AddEventWhenUsing()
        {
            var code = @"
namespace N
{
    using System;

    class C
    {
    }
}";
            var sln = CodeFactory.CreateSolution(code, MetadataReferences.FromAttributes());
            var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);
            var eventDeclaration = (EventFieldDeclarationSyntax)editor.Generator.EventDeclaration("E", SyntaxFactory.ParseTypeName("System.EventHandler"), Accessibility.Public)
                                                                      .WithSimplifiedNames();
            var containingType = editor.OriginalRoot.SyntaxTree.FindClassDeclaration("C");
            _ = editor.AddEvent(containingType, eventDeclaration);
            var expected = @"
namespace N
{
    using System;

    class C
    {
        public event EventHandler E;
    }
}";
            CodeAssert.AreEqual(expected, editor.GetChangedDocument());
        }
    }
}

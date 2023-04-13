namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UsingDirectivesInsideNamespace;

using System.Linq;
using Gu.Roslyn.Asserts;
using NUnit.Framework;

public static class DocumentEditor
{
    [Test]
    public static void WhenUnknown()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
}");

        Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.UsingDirectivesInsideNamespace(editor));
    }

    [Test]
    public static void UsingDirectiveInsideNamespace()
    {
        var editor = CreateDocumentEditor(@"
namespace N
{
    using System;
}");

        Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.UsingDirectivesInsideNamespace(editor));
    }

    [Test]
    public static void UsingDirectiveInsideAndOutsideNamespace()
    {
        var editor = CreateDocumentEditor(@"
using System;

namespace N
{
    using System.Collections;
}");

        Assert.AreEqual(CodeStyleResult.Mixed, CodeStyle.UsingDirectivesInsideNamespace(editor));
    }

    [Test]
    public static void UsingDirectiveOutsideNamespace()
    {
        var editor = CreateDocumentEditor(@"
using System;

namespace N
{
}");

        Assert.AreEqual(CodeStyleResult.No, CodeStyle.UsingDirectivesInsideNamespace(editor));
    }

    private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
    {
        return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
    }
}

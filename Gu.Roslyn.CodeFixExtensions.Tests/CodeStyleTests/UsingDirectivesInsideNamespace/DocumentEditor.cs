namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static void UsingDirectiveInsideNamespace()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
    using System;
}");

            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(editor));
        }

        [Test]
        public static void DefaultsToNull()
        {
            var editor = CreateDocumentEditor(@"
namespace N
{
}");

            Assert.AreEqual(null, CodeStyle.UsingDirectivesInsideNamespace(editor));
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

            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(editor));
        }

        [Test]
        public static void UsingDirectiveOutsideNamespace()
        {
            var editor = CreateDocumentEditor(@"
using System;

namespace N
{
}");

            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(editor));
        }

        private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
        {
            return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
        }
    }
}

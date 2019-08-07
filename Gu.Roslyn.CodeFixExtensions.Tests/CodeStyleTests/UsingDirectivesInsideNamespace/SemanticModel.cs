namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UsingDirectivesInsideNamespace
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class SemanticModel
    {
        [Test]
        public static void WhenUnknown()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }

        [Test]
        public static void UsingDirectiveInsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }

        [Test]
        public static void UsingDirectiveInsideAndOutsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;

namespace N
{
    using System.Collections;
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.Mixed, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }

        [Test]
        public static void UsingDirectiveOutsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;

namespace N
{
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(CodeStyleResult.No, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }
    }
}

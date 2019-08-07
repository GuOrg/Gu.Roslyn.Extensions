namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class SemanticModel
    {
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
            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }

        [Test]
        public static void DefaultsToNull()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(null, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
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
            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
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
            Assert.AreEqual(false, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
        }
    }
}

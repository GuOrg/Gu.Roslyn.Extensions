namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static partial class CodeStyleTests
    {
        public static class UsingDirectivesInsideNamespace
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
            public static void NoDirective()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
}");

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel));
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
}

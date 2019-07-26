namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class CodeStyleTests
    {
        public class UsingDirectivesInsideNamespace
        {
            [Test]
            public void UsingDirectiveInsideNamespace()
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
            public void NoDirective()
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
            public void UsingDirectiveInsideAndOutsideNamespace()
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
            public void UsingDirectiveOutsideNamespace()
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

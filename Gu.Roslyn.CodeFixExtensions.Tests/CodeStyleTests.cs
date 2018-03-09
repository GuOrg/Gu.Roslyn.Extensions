namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Threading;
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
namespace RoslynSandbox
{
    using System;
}");

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
            }

            [Test]
            public void NoDirective()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
}");

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
            }

            [Test]
            public void UsingDirectiveInsideAndOutsideNamespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;

namespace RoslynSandbox
{
    using System.Collections;
}");

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
            }

            [Test]
            public void UsingDirectiveOutsideNamespace()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
namespace RoslynSandbox
{
}");

                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                Assert.AreEqual(false, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
            }
        }
    }
}

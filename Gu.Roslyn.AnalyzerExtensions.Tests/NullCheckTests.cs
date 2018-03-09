namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class NullCheckTests
    {
        [TestCase("text == null")]
        [TestCase("text is null")]
        public void WhenOldStyleNullCheck(string check)
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        private readonly string text;

        public Foo(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.text = text;
        }
    }
}";
            testCode = testCode.AssertReplace("text == null", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameter = syntaxTree.FindBestMatch<ParameterSyntax>("text");
            var symbol = semanticModel.GetDeclaredSymbol(parameter);
            Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
        }

        [Test]
        public void CoalesceThrow()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        private readonly string text;

        public Foo(string text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameter = syntaxTree.FindBestMatch<ParameterSyntax>("text");
            var symbol = semanticModel.GetDeclaredSymbol(parameter);
            Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
        }
    }
}

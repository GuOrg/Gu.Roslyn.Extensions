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
        [TestCase("text != null")]
        [TestCase("text is null")]
        [TestCase("Equals(text, null)")]
        [TestCase("Equals(null, text)")]
        [TestCase("ReferenceEquals(text, null)")]
        [TestCase("ReferenceEquals(null, text)")]
        public void IsNullCheck(string check)
        {
            var code = @"
namespace N
{
    using System;

    public class C
    {
        private readonly string text;

        public C(string text, string other)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.text = text;
        }
    }
}".AssertReplace("text == null", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var expression = syntaxTree.Find<ExpressionSyntax>(check);
            Assert.AreEqual(true, NullCheck.IsNullCheck(expression, out var value));
            Assert.AreEqual("text", value.ToString());
        }

        [TestCase("text == null")]
        [TestCase("text != null")]
        [TestCase("text == null && other == null")]
        [TestCase("text is null")]
        public void IsCheckedWhenOldStyleNullCheck(string check)
        {
            var code = @"
namespace N
{
    using System;

    public class C
    {
        private readonly string text;

        public C(string text, string other)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.text = text;
        }
    }
}";
            code = code.AssertReplace("text == null", check);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameter = syntaxTree.FindParameter("text");
            var symbol = semanticModel.GetDeclaredSymbol(parameter);
            Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
        }

        [Test]
        public void IsCheckedWhenCoalesceThrow()
        {
            var code = @"
namespace N
{
    using System;

    public class C
    {
        private readonly string text;

        public C(string text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameter = syntaxTree.FindParameter("text");
            var symbol = semanticModel.GetDeclaredSymbol(parameter);
            Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
        }

        [Test]
        public void IsCheckedWhenOldStyleNullCheckOrOtherCheck()
        {
            var code = @"
namespace N
{
    using System;

    public class C
    {
        private readonly string text;

        public C(string text, string other)
        {
            if (text == null || other == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.text = text;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var parameter = syntaxTree.FindParameter("text");
            var symbol = semanticModel.GetDeclaredSymbol(parameter);
            Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
        }
    }
}

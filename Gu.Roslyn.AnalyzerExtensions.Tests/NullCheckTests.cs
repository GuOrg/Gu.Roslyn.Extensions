namespace Gu.Roslyn.AnalyzerExtensions.Tests;

using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

public static class NullCheckTests
{
    [TestCase("text == null")]
    [TestCase("null == text")]
    [TestCase("text != null")]
    [TestCase("text is null")]
    //// [TestCase("text is { }")]
    [TestCase("Equals(text, null)")]
    [TestCase("Equals(null, text)")]
    [TestCase("object.Equals(text, null)")]
    [TestCase("object.Equals(null, text)")]
    [TestCase("Object.Equals(text, null)")]
    [TestCase("Object.Equals(null, text)")]
    [TestCase("ReferenceEquals(text, null)")]
    [TestCase("ReferenceEquals(null, text)")]
    public static void IsNullCheck(string check)
    {
        var code = @"
namespace N
{
    using System;

    class C
    {
        bool M(string text) => text == null;
    }
}".AssertReplace("text == null", check);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var expression = syntaxTree.Find<ExpressionSyntax>(check);
        Assert.AreEqual(true, NullCheck.IsNullCheck(expression, default, default, out var value));
        Assert.AreEqual("text", value.ToString());

        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        Assert.AreEqual(true, NullCheck.IsNullCheck(expression, semanticModel, CancellationToken.None, out value));
        Assert.AreEqual("text", value.ToString());
    }

    [TestCase("text == null")]
    [TestCase("text != null")]
    [TestCase("text == null && other == null")]
    [TestCase("text is null")]
    public static void IsCheckedWhenOldStyleNullCheck(string check)
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
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var parameter = syntaxTree.FindParameter("text");
        var symbol = semanticModel.GetDeclaredSymbol(parameter);
        Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
    }

    [Test]
    public static void IsCheckedWhenCoalesceThrow()
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
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var parameter = syntaxTree.FindParameter("text");
        var symbol = semanticModel.GetDeclaredSymbol(parameter);
        Assert.AreEqual(true, NullCheck.IsChecked(symbol, parameter.FirstAncestor<ConstructorDeclarationSyntax>(), semanticModel, CancellationToken.None));
    }

    [Test]
    public static void IsCheckedWhenOldStyleNullCheckOrOtherCheck()
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

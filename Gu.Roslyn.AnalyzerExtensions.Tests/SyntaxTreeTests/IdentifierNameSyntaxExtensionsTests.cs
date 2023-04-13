namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests;

using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

public static class IdentifierNameSyntaxExtensionsTests
{
    [Test]
    public static void TryGetTargetProperty()
    {
        var code = @"
namespace N
{
    public class C
    {
        public object M(string text) => text.Length;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var identifierNameSyntax = (IdentifierNameSyntax)syntaxTree.FindExpression("Length");
        var method = new QualifiedProperty(new QualifiedType(typeof(string).FullName), "Length");
        Assert.AreEqual(true, identifierNameSyntax.TryGetTarget(method, semanticModel, CancellationToken.None, out var target));
        Assert.AreEqual("string.Length", target.ToString());
    }

    [TestCase("Length")]
    [TestCase("text")]
    public static void IsSymbol(string identifier)
    {
        var code = @"
namespace N
{
    public class C
    {
        public object M(string text) => text.Length;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var identifierName = (IdentifierNameSyntax)syntaxTree.FindExpression(identifier);
        Assert.AreEqual(true, semanticModel.TryGetSymbol(identifierName, CancellationToken.None, out var symbol));
        Assert.AreEqual(true, identifierName.IsSymbol(symbol, semanticModel, CancellationToken.None));
    }

    [TestCase("Length")]
    [TestCase("text")]
    [TestCase("number")]
    [TestCase("M")]
    public static void IsSymbolExtensionMethod1(string identifier)
    {
        var code = @"
namespace N
{
    public static class C
    {
        public static object M(this string text, int number) => text.Length + number;

        public static object P => string.Empty.M(1);
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var identifierName = (IdentifierNameSyntax)syntaxTree.FindExpression(identifier);
        Assert.AreEqual(true, semanticModel.TryGetSymbol(identifierName, CancellationToken.None, out var symbol));
        Assert.AreEqual(true, identifierName.IsSymbol(symbol, semanticModel, CancellationToken.None));
    }

    [Test]
    public static void IsSymbolExtensionMethod2()
    {
        var code = @"
namespace N
{
    public static class C
    {
        public static object M(this string text, int number) => text.Length + number;

        public static object P => string.Empty.M(1);
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindMethodDeclaration("M"), CancellationToken.None, out var symbol));
        var identifierName = (IdentifierNameSyntax)syntaxTree.FindExpression("M");
        Assert.AreEqual(true, identifierName.IsSymbol(symbol, semanticModel, CancellationToken.None));
    }

    [Test]
    public static void IsSymbolExtensionMethodParameter()
    {
        var code = @"
namespace N
{
    public static class C
    {
        public static object M(this string text, int number) => text.Length + number;

        public static object P => string.Empty.M(1);
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var identifierName = (IdentifierNameSyntax)syntaxTree.FindExpression("number");

        Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindMethodDeclaration("M"), CancellationToken.None, out var method));
        Assert.AreEqual(true, identifierName.IsSymbol(method.Parameters[1], semanticModel, CancellationToken.None));

        Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindInvocation("M(1)"), CancellationToken.None, out method));
        Assert.AreEqual(true, identifierName.IsSymbol(method.Parameters[0], semanticModel, CancellationToken.None));
    }
}

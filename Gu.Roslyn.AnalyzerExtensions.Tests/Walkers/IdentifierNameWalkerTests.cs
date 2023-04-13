namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers;

using System.Linq;
using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class IdentifierNameWalkerTests
{
    [Test]
    public static void TryFindWhenParameter()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C(int i)
        {
            i = 1;
        }
    }
}");
        var node = syntaxTree.FindTypeDeclaration("C");
        using var walker = IdentifierNameWalker.Borrow(node);
        CollectionAssert.AreEqual(new[] { "i" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));
        Assert.AreEqual(true, walker.TryFind("i", out var match));
        Assert.AreEqual("i", match.Identifier.ValueText);
        Assert.AreEqual(false, walker.TryFind("missing", out _));
    }

    [Test]
    public static void ForWhenParameter()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C(int i)
        {
            i = 1;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindParameter("i"), CancellationToken.None, out var parameter));
        using var walker = IdentifierNameWalker.For(parameter, semanticModel, CancellationToken.None);
        CollectionAssert.AreEqual(new[] { "i" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));
        Assert.AreEqual(true, walker.TryFind("i", out var match));
        Assert.AreEqual("i", match.Identifier.ValueText);
        Assert.AreEqual(false, walker.TryFind("missing", out _));
    }

    [Test]
    public static void ForWhenLocal()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 0;
            i = 1;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        Assert.AreEqual(true, semanticModel.TryGetSymbol(syntaxTree.FindVariableDeclaration("i"), CancellationToken.None, out ILocalSymbol local));
        using var walker = IdentifierNameWalker.For(local, semanticModel, CancellationToken.None);
        CollectionAssert.AreEqual(new[] { "i" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));
        Assert.AreEqual(true, walker.TryFind("i", out var match));
        Assert.AreEqual("i", match.Identifier.ValueText);
        Assert.AreEqual(false, walker.TryFind("missing", out _));
    }

    [Test]
    public static void TryFindWhenProperty()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P { get; }

        public void M()
        {
            var p = this.P;
        }
    }
}");
        var node = syntaxTree.FindMethodDeclaration("M");
        using var walker = IdentifierNameWalker.Borrow(node);
        CollectionAssert.AreEqual(new[] { "var", "P" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));

        Assert.AreEqual(true, walker.TryFind("P", out var match));
        Assert.AreEqual("P", match.Identifier.ValueText);

        Assert.AreEqual(false, walker.TryFind("missing", out _));
    }

    [Test]
    public static void TryFindFirst()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C(int i)
        {
            i = 1;
            i = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindParameter("int i"), CancellationToken.None);
        var node = syntaxTree.FindTypeDeclaration("C");

        Assert.AreEqual(true, IdentifierNameWalker.TryFindFirst(node, symbol, semanticModel, CancellationToken.None, out var match));
        Assert.AreEqual("i = 1", match.Parent.ToString());

        using var walker = IdentifierNameWalker.Borrow(node);
        Assert.AreEqual(true, walker.TryFindFirst(symbol, semanticModel, CancellationToken.None, out match));
        Assert.AreEqual("i = 1", match.Parent.ToString());
    }

    [Test]
    public static void TryFindLast()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C(int i)
        {
            i = 1;
            i = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbolSafe(syntaxTree.FindParameter("int i"), CancellationToken.None);
        var node = syntaxTree.FindTypeDeclaration("C");

        Assert.AreEqual(true, IdentifierNameWalker.TryFindLast(node, symbol, semanticModel, CancellationToken.None, out var match));
        Assert.AreEqual("i = 2", match.Parent.ToString());

        using var walker = IdentifierNameWalker.Borrow(node);
        Assert.AreEqual(true, walker.TryFindLast(symbol, semanticModel, CancellationToken.None, out match));
        Assert.AreEqual("i = 2", match.Parent.ToString());
    }
}

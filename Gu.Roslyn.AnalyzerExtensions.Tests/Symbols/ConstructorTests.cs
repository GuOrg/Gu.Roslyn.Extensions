namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols;

using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class ConstructorTests
{
    [TestCase(Search.TopLevel)]
    [TestCase(Search.Recursive)]
    public static void TryFindDefaultSimple(Search search)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal class C
    {
        internal C()
        {
        }

        internal C(int intValue)
            : this()
        {
        }

        internal C(string textValue)
            : this(1)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
        Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out var ctor));
        Assert.AreEqual("N.C.C()", ctor.ToString());
    }

    [TestCase(Search.TopLevel)]
    [TestCase(Search.Recursive)]
    public static void TryFindDefaultWithBaseAndDefault(Search search)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class CBase
    {
        public CBase()
        {
        }
    }

    internal class C : CBase
    {
        internal C()
        {
        }

        internal C(int intValue)
            : this()
        {
        }

        internal C(string textValue)
            : this(1)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
        Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out var ctor));
        Assert.AreEqual("N.C.C()", ctor.ToString());

        type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("class CBase"));
        Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out ctor));
        Assert.AreEqual("N.CBase.CBase()", ctor.ToString());
    }

    [TestCase(Search.TopLevel, null)]
    [TestCase(Search.Recursive, "N.CBase.CBase()")]
    public static void TryFindDefaultWithBase(Search search, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class CBase
    {
        public CBase()
        {
        }
    }

    internal class C : CBase
    {
        internal C(int intValue)
            : this()
        {
        }

        internal C(string textValue)
            : this(1)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
        Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, search, out var ctor));
        Assert.AreEqual(expected, ctor?.ToString());
    }

    [TestCase(Search.TopLevel, null)]
    [TestCase(Search.Recursive, "N.CBaseBase.CBaseBase()")]
    public static void TryFindDefaultWithBaseWithGap(Search search, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class CBaseBase
    {
        public CBaseBase()
        {
        }
    }

    class CBase : CBaseBase
    {
    }

    internal class C : CBase
    {
        internal C(int intValue)
            : this()
        {
        }

        internal C(string textValue)
            : this(1)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
        Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, search, out var ctor));
        Assert.AreEqual(expected, ctor?.ToString());
    }
}

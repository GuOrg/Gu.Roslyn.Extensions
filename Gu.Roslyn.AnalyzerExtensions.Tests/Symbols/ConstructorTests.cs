namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class ConstructorTests
    {
        [TestCase(Recursive.No)]
        [TestCase(Recursive.Yes)]
        public static void TryFindDefaultSimple(Recursive recursive)
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("C"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, recursive, out var ctor));
            Assert.AreEqual("N.C.C()", ctor.ToString());
        }

        [TestCase(Recursive.No)]
        [TestCase(Recursive.Yes)]
        public static void TryFindDefaultWithBaseAndDefault(Recursive recursive)
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, recursive, out var ctor));
            Assert.AreEqual("N.C.C()", ctor.ToString());

            type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("class CBase"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, recursive, out ctor));
            Assert.AreEqual("N.CBase.CBase()", ctor.ToString());
        }

        [TestCase(Recursive.No, null)]
        [TestCase(Recursive.Yes, "N.CBase.CBase()")]
        public static void TryFindDefaultWithBase(Recursive recursive, string expected)
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
            Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, recursive, out var ctor));
            Assert.AreEqual(expected, ctor?.ToString());
        }

        [TestCase(Recursive.No, null)]
        [TestCase(Recursive.Yes, "N.CBaseBase.CBaseBase()")]
        public static void TryFindDefaultWithBaseWithGap(Recursive recursive, string expected)
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
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class C : CBase"));
            Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, recursive, out var ctor));
            Assert.AreEqual(expected, ctor?.ToString());
        }
    }
}

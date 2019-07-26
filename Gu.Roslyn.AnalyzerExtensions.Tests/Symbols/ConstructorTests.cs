namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class ConstructorTests
    {
        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public void TryFindDefaultSimple(Search search)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    internal class Foo
    {
        internal Foo()
        {
        }

        internal Foo(int intValue)
            : this()
        {
        }

        internal Foo(string textValue)
            : this(1)
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("Foo"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out var ctor));
            Assert.AreEqual("N.Foo.Foo()", ctor.ToString());
        }

        [TestCase(Search.TopLevel)]
        [TestCase(Search.Recursive)]
        public void TryFindDefaultWithBaseAndDefault(Search search)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class FooBase
    {
        public FooBase()
        {
        }
    }

    internal class Foo : FooBase
    {
        internal Foo()
        {
        }

        internal Foo(int intValue)
            : this()
        {
        }

        internal Foo(string textValue)
            : this(1)
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class Foo : FooBase"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out var ctor));
            Assert.AreEqual("N.Foo.Foo()", ctor.ToString());

            type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("class FooBase"));
            Assert.AreEqual(true, Constructor.TryFindDefault(type, search, out ctor));
            Assert.AreEqual("N.FooBase.FooBase()", ctor.ToString());
        }

        [TestCase(Search.TopLevel, null)]
        [TestCase(Search.Recursive, "N.FooBase.FooBase()")]
        public void TryFindDefaultWithBase(Search search, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class FooBase
    {
        public FooBase()
        {
        }
    }

    internal class Foo : FooBase
    {
        internal Foo(int intValue)
            : this()
        {
        }

        internal Foo(string textValue)
            : this(1)
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class Foo : FooBase"));
            Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, search, out var ctor));
            Assert.AreEqual(expected, ctor?.ToString());
        }

        [TestCase(Search.TopLevel, null)]
        [TestCase(Search.Recursive, "N.FooBaseBase.FooBaseBase()")]
        public void TryFindDefaultWithBaseWithGap(Search search, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    class FooBaseBase
    {
        public FooBaseBase()
        {
        }
    }

    class FooBase : FooBaseBase
    {
    }

    internal class Foo : FooBase
    {
        internal Foo(int intValue)
            : this()
        {
        }

        internal Foo(string textValue)
            : this(1)
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var type = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("internal class Foo : FooBase"));
            Assert.AreEqual(expected != null, Constructor.TryFindDefault(type, search, out var ctor));
            Assert.AreEqual(expected, ctor?.ToString());
        }
    }
}

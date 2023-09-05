namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.ExecutionWalkerTests;

using System.Linq;
using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class Constructor
{
    [TestCase(SearchScope.Member)]
    [TestCase(SearchScope.Instance)]
    [TestCase(SearchScope.Type)]
    [TestCase(SearchScope.Recursive)]
    public static void ExplicitParameterless(SearchScope scope)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 1;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindConstructorDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual("1", walker.Literals.Single().ToString());
    }

    [TestCase(SearchScope.Member, "1, 2")]
    [TestCase(SearchScope.Instance, "2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void StaticBeforeExplicitParameterless(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        static C()
        {
            var i = 1;
        }

        public C()
        {
            var i = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindTypeDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "1, 2")]
    [TestCase(SearchScope.Instance, "2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void StaticBeforeExplicitParameterlessWhenNotDocumentOrder(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            var i = 2;
        }

        static C()
        {
            var i = 1;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindTypeDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "1, 3")]
    [TestCase(SearchScope.Instance, "1, 2, 3")]
    [TestCase(SearchScope.Type, "1, 2, 3")]
    [TestCase(SearchScope.Recursive, "1, 2, 3")]
    public static void ChainedThis(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
            : this(1)
        {
            var j = 3;
        }

        public C(int _)
        {
            var i = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindConstructorDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "2")]
    [TestCase(SearchScope.Instance, "1, 2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void ImplicitBaseParameterLess(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class CBase
    {
        public CBase()
        {
            var i = 1;
        }
    }

    public class C : CBase
    {
        public C()
        {
            var j = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindConstructorDeclaration("public C()");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "2")]
    [TestCase(SearchScope.Instance, "1, 2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void ExplicitBaseParameterLess(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class CBase
    {
        public CBase()
        {
            var i = 1;
        }
    }

    public class C : CBase
    {
        public C()
            : base()
        {
            var j = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindConstructorDeclaration("public C()");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "1, 2")]
    [TestCase(SearchScope.Instance, "1, 2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void FieldInitializerBeforeConstructor(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        private readonly int value = 1;

        public C()
        {
            this.value = 2;
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindClassDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "1, 2")]
    [TestCase(SearchScope.Instance, "1, 2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void FieldInitializerBeforeConstructorWhenNotDocumentOrder(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public C()
        {
            this.value = 2;
        }

        private readonly int value = 1;
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindClassDeclaration("C");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase("new C()", SearchScope.Member, "3")]
    [TestCase("new  ()", SearchScope.Member, "3")]
    [TestCase("new C()", SearchScope.Instance, "1, 2, 3")]
    [TestCase("new  ()", SearchScope.Instance, "1, 2, 3")]
    [TestCase("new C()", SearchScope.Type, "1, 2, 3")]
    [TestCase("new  ()", SearchScope.Type, "1, 2, 3")]
    [TestCase("new C()", SearchScope.Recursive, "1, 2, 3")]
    [TestCase("new  ()", SearchScope.Recursive, "1, 2, 3")]
    public static void PropertyInitializerBeforeConstructor(string objectCreation, SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C() { Value2 = 3 };

        public C()
        {
            this.Value1 = 2;
        }

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}".AssertReplace("new C()", objectCreation));
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindExpression("new C() { Value2 = 3 }".AssertReplace("new C()", objectCreation));
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase("new C()", SearchScope.Member, "2")]
    [TestCase("new  ()", SearchScope.Member, "2")]
    [TestCase("new C()", SearchScope.Instance, "1, 2")]
    [TestCase("new  ()", SearchScope.Instance, "1, 2")]
    [TestCase("new C()", SearchScope.Type, "1, 2")]
    [TestCase("new  ()", SearchScope.Type, "1, 2")]
    [TestCase("new C()", SearchScope.Recursive, "1, 2")]
    [TestCase("new  ()", SearchScope.Recursive, "1, 2")]
    public static void PropertyInitializerDefaultCtor(string objectCreation, SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C() { Value2 = 2 };

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}".AssertReplace("new C()", objectCreation));
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindExpression("new C() { Value2 = 2 }".AssertReplace("new C()", objectCreation));
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }

    [TestCase(SearchScope.Member, "2")]
    [TestCase(SearchScope.Instance, "1, 2")]
    [TestCase(SearchScope.Type, "1, 2")]
    [TestCase(SearchScope.Recursive, "1, 2")]
    public static void PropertyInitializerBeforeDefaultCtorObjectInitializer(SearchScope scope, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C { Value2 = 2 };

        public int Value1 { get; set; } = 1;

        public int Value2 { get; set; }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindExpression("new C { Value2 = 2 }");
        using var walker = LiteralWalker.Borrow(node, scope, semanticModel, CancellationToken.None);
        Assert.AreEqual(expected, string.Join(", ", walker.Literals));
    }
}

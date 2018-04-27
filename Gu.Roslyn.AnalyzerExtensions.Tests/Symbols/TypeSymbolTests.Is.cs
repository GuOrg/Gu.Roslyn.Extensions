namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class TypeSymbolTests
    {
        public class Is
        {
            [TestCase("int value1, int value2")]
            [TestCase("int value1, int? value2")]
            [TestCase("int value1, System.IComparable value2")]
            [TestCase("int value1, System.IComparable<int> value2")]
            [TestCase("int value1, object value2")]
            public void WhenTrue(string parameters)
            {
                var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int value1, int value2)
        {
        }
    }
}";
                code = code.AssertReplace("int value1, int value2", parameters);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var ctor = semanticModel.GetDeclaredSymbol(syntaxTree.FindConstructorDeclaration("Foo"));
                var type1 = ctor.Parameters[0].Type;
                var type2 = ctor.Parameters[1].Type;
                Assert.AreEqual(true, type1.Is(type2));
            }

            [Test]
            public void Inheritance()
            {
                var code = @"
namespace RoslynSandbox
{
    class A
    {
    }

    class B : A
    {
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var a = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("A"));
                var b = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("B"));
                Assert.AreEqual(false, a.Is(b));
                Assert.AreEqual(true, b.Is(a));
            }

            [Test]
            public void InheritsGeneric()
            {
                var code = @"
namespace RoslynSandbox
{
    class A<T>
    {
    }

    class B : A<int>
    {
    }
}";
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var a = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("A"));
                var b = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("B"));
                Assert.AreEqual(false, a.Is(b));
                Assert.AreEqual(true, b.Is(a));
            }
        }
    }
}

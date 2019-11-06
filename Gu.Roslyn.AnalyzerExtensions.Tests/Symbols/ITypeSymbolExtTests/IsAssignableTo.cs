namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols.ITypeSymbolExtTests
{
    using System;
    using System.Collections.Generic;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class IsAssignableTo
    {
        [TestCase("int value1, int value2", true)]
        [TestCase("int value1, int? value2", true)]
        [TestCase("int value1, double value2", true)]
        [TestCase("int value1, System.IComparable value2", true)]
        [TestCase("int value1, System.IComparable<int> value2", true)]
        [TestCase("int value1, object value2", true)]
        [TestCase("System.Collections.Generic.IEnumerable<int> value1, System.Collections.IEnumerable value2", true)]
        [TestCase("System.Collections.Generic.IEnumerable<int> value1, System.Collections.Generic.IEnumerable<int> value2", true)]
        public static void TypeSymbols(string parameters, bool expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int value1, int value2)
        {
        }
    }
}";
            code = code.AssertReplace("int value1, int value2", parameters);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var ctor = semanticModel.GetDeclaredSymbol(syntaxTree.FindConstructorDeclaration("C"));
            var type1 = ctor.Parameters[0].Type;
            var type2 = ctor.Parameters[1].Type;
            Assert.AreEqual(expected, type1.IsAssignableTo(type2, compilation));
        }

        [TestCase("int", typeof(int))]
        [TestCase("int", typeof(int?))]
        [TestCase("int", typeof(double))]
        [TestCase("int", typeof(IComparable))]
        [TestCase("int", typeof(IComparable<int>))]
        [TestCase("int", typeof(object))]
        [TestCase("int[]", typeof(int[]))]
        [TestCase("System.Collections.Generic.IEnumerable<int>", typeof(System.Collections.IEnumerable))]
        [TestCase("System.Collections.Generic.IEnumerable<int>", typeof(System.Collections.Generic.IEnumerable<int>))]
        public static void QualifiedTypeFromType(string typeString, Type destination)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value { get; }
    }
}".AssertReplace("int", typeString);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Value"));
            var source = property.Type;
            var qualifiedType = QualifiedType.FromType(destination);
            Assert.AreEqual(true, source.IsAssignableTo(qualifiedType, compilation));
        }

        [TestCase("System.Collections.Generic.IEnumerable<int>", typeof(System.Collections.IEnumerable))]
        [TestCase("System.Collections.Generic.IEnumerable<int>", typeof(System.Collections.Generic.IEnumerable<int>))]
        public static void QualifiedTypeFromTypeNoMetadataReferences(string typeString, Type destination)
        {
            var code = @"
namespace N
{
    public class C
    {
        public int Value { get; }
    }
}".AssertReplace("int", typeString);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, new List<MetadataReference>());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var property = semanticModel.GetDeclaredSymbol(syntaxTree.FindPropertyDeclaration("Value"));
            var source = property.Type;
            var qualifiedType = QualifiedType.FromType(destination);
            Assert.AreEqual(false, source.IsAssignableTo(qualifiedType, compilation));
        }

        [TestCase("int value", "System.Int32")]
        [TestCase("int value", "System.IComparable")]
        [TestCase("System.Collections.Generic.IEnumerable<int> value", "System.Collections.IEnumerable")]
        public static void WhenTrueIsAssignableToQualifiedType(string parameters, string typeName)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int value)
        {
        }
    }
}";
            code = code.AssertReplace("int value", parameters);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation =
                CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var ctor = semanticModel.GetDeclaredSymbol(syntaxTree.FindConstructorDeclaration("C"));
            var type = ctor.Parameters[0].Type;
            var qualifiedType = new QualifiedType(typeName);
            Assert.AreEqual(true, type.IsAssignableTo(qualifiedType, compilation));
        }

        [Test]
        public static void Inheritance()
        {
            var code = @"
namespace N
{
    class A
    {
    }

    class B : A
    {
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation =
                CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var a = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("A"));
            var b = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("B"));
            Assert.AreEqual(false, a.IsAssignableTo(b, compilation));
            Assert.AreEqual(true, b.IsAssignableTo(a, compilation));
        }

        [Test]
        public static void InheritsGeneric()
        {
            var code = @"
namespace N
{
    class A<T>
    {
    }

    class B : A<int>
    {
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation =
                CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var a = semanticModel.GetTypeInfo(syntaxTree.Find<BaseTypeSyntax>("A<int>").Type).Type;
            var b = semanticModel.GetDeclaredSymbol(syntaxTree.FindClassDeclaration("B"));
            Assert.AreEqual(false, a.IsAssignableTo(b, compilation));
            Assert.AreEqual(true, b.IsAssignableTo(a, compilation));
        }

        [TestCase("int value", false)]
        [TestCase("System.Threading.Tasks.Task value", true)]
        public static void IsAwaitable(string parameter, bool expected)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int value)
        {
        }
    }
}";
            code = code.AssertReplace("int value", parameter);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation =
                CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var ctor = semanticModel.GetDeclaredSymbol(syntaxTree.FindConstructorDeclaration("C"));
            var type = ctor.Parameters[0].Type;
            Assert.AreEqual(expected, type.IsAwaitable());
        }
    }
}

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable CoVariantArrayConversion
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class INamedTypeSymbolExtensionsTests
    {
        [TestCase(typeof(int))]
        [TestCase(typeof(Nullable<>))]
        [TestCase(typeof(IEnumerable))]
        [TestCase(typeof(IEnumerable<>))]
        [TestCase(typeof(List<>))]
        [TestCase(typeof(Dictionary<,>))]
        [TestCase(typeof(Nested))]
        public void FullName(Type type)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
    }
}");
            type = Type.GetType(type.FullName);
            Assert.NotNull(type);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.Transitive(type));
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeSymbol = semanticModel.Compilation.GetTypeByMetadataName(type.FullName);
            Assert.AreEqual(type.FullName, typeSymbol.FullName());
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(IEnumerable<int>))]
        [TestCase(typeof(IEnumerable<IEnumerable<int>>))]
        [TestCase(typeof(IEnumerable<IEnumerable<IEnumerable<int>>>))]
        [TestCase(typeof(IEnumerable<INamedTypeSymbolExtensionsTests>))]
        [TestCase(typeof(IEnumerable<Nested>))]
        [TestCase(typeof(IEnumerable<IEnumerable<Nested>>))]
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(Dictionary<int, string>))]
        public void FullNameWithTypeArgs(Type type)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                @"
namespace N
{
    public class C
    {
    }
}");
            type = Type.GetType(type.FullName);
            Assert.NotNull(type);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.Transitive(this.GetType()));
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var typeSymbol = GetTypeSymbol(type);
            Assert.AreEqual(type.FullName, typeSymbol.FullName());

            INamedTypeSymbol GetTypeSymbol(Type current)
            {
                return current.IsGenericType
                    ? semanticModel.Compilation.GetTypeByMetadataName(current.GetGenericTypeDefinition().FullName)
                                   .Construct(current.GenericTypeArguments.Select(GetTypeSymbol).ToArray())
                    : semanticModel.Compilation.GetTypeByMetadataName(current.FullName);
            }
        }

        public class Nested
        {
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class ArgumentSyntaxExtTests
    {
        [TestCase("\"text\"",                  "text")]
        [TestCase("Const",                     "const text")]
        [TestCase("string.Empty",              "")]
        [TestCase("String.Empty",              "")]
        [TestCase("null",                      null)]
        [TestCase("nameof(Foo)",               "Foo")]
        [TestCase("nameof(RoslynSandbox.Foo)", "Foo")]
        [TestCase("nameof(Foo<int>)",          "Foo")]
        [TestCase("nameof(NestedFoo<int>)",    "NestedFoo")]
        [TestCase("nameof(Bar)",               "Bar")]
        [TestCase("nameof(this.Bar)",          "Bar")]
        [TestCase("(string)null",              null)]
        public void TryGetStringValue(string code, string expected)
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public const string Const = ""const text"";

        public Foo()
        {
            Bar(""text"");
        }

        private void Bar(string arg) { }

        public class NestedFoo<T> { }
    }

    public class Foo<T> { }
}".AssertReplace("\"text\"", code);
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation =
            CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindArgument(code);
            Assert.AreEqual(true,
            invocation.TryGetStringValue(semanticModel, CancellationToken.None, out var name));
            Assert.AreEqual(expected, name);
        }

        [Test]
        public void TryGetTypeofValue()
        {
            var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo()
        {
            Bar(typeof(int));
        }

        private void Bar(Type arg)
        {
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation =
            CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindArgument("typeof(int)");
            Assert.AreEqual(true,  invocation.TryGetTypeofValue(semanticModel, CancellationToken.None, out var type));
            Assert.AreEqual("int", type.ToString());
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class SemanticModelExtTests
    {
        public class IsRepresentationPreservingConversion
        {
            [TestCase("Cast<int>(1)")]
            [TestCase("Cast<int?>(1)")]
            [TestCase("Cast<int?>(null)")]
            [TestCase("Cast<System.IComparable>(1)")]
            [TestCase("Cast<System.IComparable<int>>(1)")]
            [TestCase("Cast<object>(1)")]
            [TestCase("Cast<object>(new object())")]
            [TestCase("Cast<System.Collections.IEnumerable>(\"abc\")")]
            public void TrueWhen(string call)
            {
                var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var cast = Cast<int>(1);
        }

        private static T Cast<T>(object o) => (T) o;
    }
}";
                code = code.AssertReplace("Cast<int>(1)", call);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation(call);
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                var expressionSyntax = invocation.ArgumentList.Arguments[0].Expression;
                Assert.AreEqual(true, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType, CancellationToken.None));
            }

            [TestCase("Cast<int?>(1)")]
            [TestCase("Cast<int?>(null)")]
            [TestCase("Cast<int?>(arg)")]
            public void TrueWhenNullable(string call)
            {
                var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int? arg)
        {
            var cast = Cast<int?>(arg);
        }

        private static T Cast<T>(object o) => (T) o;
    }
}";
                code = code.AssertReplace("Cast<int?>(arg)", call);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation(call);
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                var expressionSyntax = invocation.ArgumentList.Arguments[0].Expression;
                Assert.AreEqual(true, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType, CancellationToken.None));
            }

            [TestCase("Cast<int>(1.0)")]
            [TestCase("Cast<double>(1)")]
            public void FalseWhen(string call)
            {
                var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo()
        {
            var cast = Cast<int>(1);
        }

        private static T Cast<T>(object o) => (T) o;
    }
}";
                code = code.AssertReplace("Cast<int>(1)", call);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var invocation = syntaxTree.FindInvocation(call);
                var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
                var expressionSyntax = invocation.ArgumentList.Arguments[0].Expression;
                Assert.AreEqual(false, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType, CancellationToken.None));
            }
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class IsRepresentationPreservingConversion
    {
        [TestCase("Cast<int>(1)")]
        [TestCase("Cast<int?>(1)")]
        [TestCase("Cast<int?>(null)")]
        [TestCase("Cast<double>(1.0)")]
        [TestCase("Cast<double>(1d)")]
        [TestCase("Cast<double>((double)1)")]
        [TestCase("Cast<double>((System.Double)1)")]
        [TestCase("Cast<System.IComparable>(1)")]
        [TestCase("Cast<System.IComparable<int>>(1)")]
        [TestCase("Cast<object>(1)")]
        [TestCase("Cast<System.StringComparison>(System.StringComparison.CurrentCulture)")]
        [TestCase("Cast<System.StringComparison>((object)System.StringComparison.CurrentCulture)")]
        [TestCase("Cast<E>(E.M1)")]
        [TestCase("Cast<object>(new object())")]
        [TestCase("Cast<System.Collections.IEnumerable>(\"abc\")")]
        public static void TrueWhen(string call)
        {
            var e = @"
namespace N
{
    public enum E
    {
        M1,
        M2,
    }
}";
            var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            var cast = Cast<int>(1);
        }

        private static T Cast<T>(object o) => (T) o;
    }
}";
            code = code.AssertReplace("Cast<int>(1)", call);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree, CSharpSyntaxTree.ParseText(e) }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation(call);
            var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
            var expressionSyntax = invocation.ArgumentList.Arguments[0].Expression;
            Assert.AreEqual(true, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType));
        }

        [TestCase("Cast<int?>(1)")]
        [TestCase("Cast<int?>((object)1)")]
        [TestCase("Cast<int?>(null)")]
        [TestCase("Cast<int?>((object)null)")]
        [TestCase("Cast<int?>(arg)")]
        public static void TrueWhenNullable(string call)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C(int? arg)
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
            Assert.AreEqual(true, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType));
        }

        [TestCase("Cast<int>(1.0)")]
        [TestCase("Cast<int>(null)")]
        [TestCase("Cast<int>((object)null)")]
        [TestCase("Cast<int>((object)1.0)")]
        [TestCase("Cast<int?>((object)1.0)")]
        [TestCase("Cast<double>(1)")]
        [TestCase("Cast<double?>(1)")]
        public static void FalseWhen(string call)
        {
            var code = @"
namespace N
{
    public class C
    {
        public C()
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
            Assert.AreEqual(false, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType));
        }
    }
}

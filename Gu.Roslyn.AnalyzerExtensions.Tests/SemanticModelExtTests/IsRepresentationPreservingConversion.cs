namespace Gu.Roslyn.AnalyzerExtensions.Tests.SemanticModelExtTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class IsRepresentationPreservingConversion
    {
        [TestCase("Cast<int>(1)",                                                                                                  true)]
        [TestCase("Cast<int>(null)",                                                                                               false)]
        [TestCase("Cast<int>((object)null)",                                                                                       false)]
        [TestCase("Cast<int>(1.0)",                                                                                                false)]
        [TestCase("Cast<int>((object)1.0)",                                                                                        false)]
        [TestCase("Cast<int>(new object())",                                                                                       false)]
        [TestCase("Cast<int>((object)1)",                                                                                          true)]
        [TestCase("Cast<int?>(1)",                                                                                                 true)]
        [TestCase("Cast<int?>((object)1)",                                                                                         true)]
        [TestCase("Cast<int?>(null)",                                                                                              true)]
        [TestCase("Cast<int?>((object)null)",                                                                                      true)]
        [TestCase("Cast<int?>(1.0)",                                                                                               false)]
        [TestCase("Cast<int?>((object)1.0)",                                                                                       false)]
        [TestCase("Cast<int>(n)",                                                                                                  true)]
        [TestCase("Cast<int?>(n)",                                                                                                 true)]
        [TestCase("Cast<int?>(nullableInt)",                                                                                       true)]
        [TestCase("Cast<double>(1.0)",                                                                                             true)]
        [TestCase("Cast<double>(1d)",                                                                                              true)]
        [TestCase("Cast<double>(1)",                                                                                               false)]
        [TestCase("Cast<double>((double)1)",                                                                                       true)]
        [TestCase("Cast<double>((System.Double)1)",                                                                                true)]
        [TestCase("Cast<double?>(1)",                                                                                              false)]
        [TestCase("Cast<System.IComparable>(1)",                                                                                   true)]
        [TestCase("Cast<System.IComparable<int>>(1)",                                                                              true)]
        [TestCase("Cast<object>(1)",                                                                                               true)]
        [TestCase("Cast<System.StringComparison>(System.StringComparison.CurrentCulture)",                                         true)]
        [TestCase("Cast<System.StringComparison>((object)System.StringComparison.CurrentCulture)",                                 true)]
        [TestCase("Cast<E>(E.M1)",                                                                                                 true)]
        [TestCase("Cast<E?>(E.M1)",                                                                                                true)]
        [TestCase("Cast<object>(new object())",                                                                                    true)]
        [TestCase("Cast<System.Collections.IEnumerable>(\"abc\")",                                                                 true)]
        [TestCase("Cast<System.Collections.IEnumerable>(new ints[0])",                                                             true)]
        [TestCase("Cast<System.Collections.IEnumerable>(new System.Collections.ObjectModel.ObservableCollection<int>())",          true)]
        [TestCase("Cast<System.Collections.IEnumerable>((object)new System.Collections.ObjectModel.ObservableCollection<int>())",  true)]
        [TestCase("Cast<System.Collections.IEnumerable?>(\"abc\")",                                                                true)]
        [TestCase("Cast<System.Collections.IEnumerable?>(new ints[0])",                                                            true)]
        [TestCase("Cast<System.Collections.IEnumerable?>(new System.Collections.ObjectModel.ObservableCollection<int>())",         true)]
        [TestCase("Cast<System.Collections.IEnumerable?>((object)new System.Collections.ObjectModel.ObservableCollection<int>())", true)]
        public static void When(string call, bool expected)
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
        public C(int n, int? nullableInt)
        {
            _ = Cast<int>(1);
        }

        private static T Cast<T>(object o) => (T) o;
    }
}".AssertReplace("Cast<int>(1)", call);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create(
                "test",
                new[] { syntaxTree, CSharpSyntaxTree.ParseText(e) },
                MetadataReferences.FromAttributes(),
                CodeFactory.DllCompilationOptions.WithNullableContextOptions(NullableContextOptions.Enable));
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation(call);
            var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
            var expressionSyntax = invocation.ArgumentList.Arguments[0].Expression;
            Assert.AreEqual(expected, semanticModel.IsRepresentationPreservingConversion(expressionSyntax, method.ReturnType));
        }
    }
}

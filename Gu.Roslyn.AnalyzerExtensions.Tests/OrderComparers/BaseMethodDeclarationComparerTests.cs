namespace Gu.Roslyn.AnalyzerExtensions.Tests.OrderComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class BaseMethodDeclarationComparerTests
    {
        private static readonly SyntaxTree SyntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo : IFoo
    {
        public static int PublicStatic() => 1;

        public int Public() => 1;

        object IFoo.Public() => 1;

        internal static int InternalStatic() => 1;

        internal int Internal() => 1;

        private static int PrivateStatic() => 1;

        private int Private() => 1;
    }

    public interface IFoo
    {
        object Public();
    }
}");

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases().ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public void Compare(BaseMethodDeclarationSyntax x, BaseMethodDeclarationSyntax y)
        {
            Assert.AreEqual(-1, BaseMethodDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, BaseMethodDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, BaseMethodDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, BaseMethodDeclarationComparer.Compare(y, y));
        }

        private static IEnumerable<TestCaseData> CreateTestCases()
        {
            var foo = SyntaxTree.FindClassDeclaration("Foo");
            foreach (var member1 in foo.Members)
            {
                foreach (var member2 in foo.Members)
                {
                    if (member1.SpanStart < member2.SpanStart)
                    {
                        yield return new TestCaseData(member1, member2);
                    }
                }
            }
        }
    }
}

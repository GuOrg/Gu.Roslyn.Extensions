namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class FieldDeclarationComparerTests
    {
        private static readonly SyntaxTree SyntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public const int PublicConst1 = 1;
        public const int PublicConst2 = PublicConst1;
        public static readonly int PublicStatic1 = 1;
        public static readonly int PublicStatic2 = PublicStatic1;
        public readonly int PublicReadonly1 = 3;
        public int Public1 = 3;

        private const int PrivateConst1 = 1;
        private const int PrivateConst2 = PrivateConst1;
        private static readonly int PrivateStatic1 = 1;
        private static readonly int PrivateStatic2 = PrivateStatic1;
        private readonly int PrivateReadonly1 = 1;
        private readonly int PrivateReadonly2 = 2;
        private int Private1;
        private int Private2;
        private int Private3 = 3;
        private int Private4 = 4;
        int Private5;
    }
}");

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases().ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public void Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
        }

        [Test]
        public void InitializedWithOther()
        {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public const int PublicConst1 = PublicConst2;
        public const int PublicConst2 = 3;
    }
}");
            var x = syntaxTree.FindFieldDeclaration("public const int PublicConst1 = PublicConst2");
            var y = syntaxTree.FindFieldDeclaration("public const int PublicConst2 = 3");
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
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

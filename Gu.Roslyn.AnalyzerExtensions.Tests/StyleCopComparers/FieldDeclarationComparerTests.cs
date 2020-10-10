namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class FieldDeclarationComparerTests
    {
        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases(CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
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
}")).ToArray();

        private static readonly IReadOnlyList<TestCaseData> BackingFieldSource = CreateTestCases(CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
    {
        private int value1;
        private int value2;

        public int Value1
        {
            get => value1;
            set => value1 = value;
        }

        public int Value2
        {
            get => value2;
            set => value2 = value;
        }
    }
}")).ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public static void Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public static void MemberDeclarationComparerCompare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        [Test]
        public static void InitializedWithOther()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
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

        [TestCaseSource(nameof(BackingFieldSource))]
        public static void BackingField(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
        }

        public static IEnumerable<TestCaseData> CreateTestCases(SyntaxTree tree)
        {
            var c = tree.FindClassDeclaration("C");
            foreach (var member1 in c.Members.OfType<FieldDeclarationSyntax>())
            {
                foreach (var member2 in c.Members.OfType<FieldDeclarationSyntax>())
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

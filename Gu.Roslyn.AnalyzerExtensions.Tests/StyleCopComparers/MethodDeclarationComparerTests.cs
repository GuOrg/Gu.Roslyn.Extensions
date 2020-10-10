namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class MethodDeclarationComparerTests
    {
        private static readonly FieldInfo PositionField = typeof(SyntaxNode).GetField("<Position>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases(
            @"
namespace N
{
    public class C : IC
    {
        public static int PublicStatic() => 1;

        public int Public() => 1;

        object IC.Public() => 1;

        internal static int InternalStatic() => 1;

        internal int Internal() => 1;

        private static int PrivateStatic() => 1;

        private int Private() => 1;
    }

    public interface IC
    {
        object Public();
    }
}",
            stripLines: false);

        [TestCaseSource(nameof(TestCaseSource))]
        public static void Compare(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MethodDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MethodDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MethodDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MethodDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public static void MemberDeclarationComparerCompare(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        public static TestCaseData[] CreateTestCases(string code, bool stripLines)
        {
            var tree = CSharpSyntaxTree.ParseText(code);

            return All().Select(x =>
            {
                if (stripLines)
                {
                    PositionField!.SetValue(x.Item1, 1);
                    PositionField.SetValue(x.Item2, 1);
                }

                return new TestCaseData(x.Item1, x.Item2);
            }).ToArray();

            List<(FieldDeclarationSyntax, FieldDeclarationSyntax)> All()
            {
                var pairs = new List<(FieldDeclarationSyntax, FieldDeclarationSyntax)>();
                var c = tree.FindClassDeclaration("C");
                foreach (var member1 in c.Members.OfType<FieldDeclarationSyntax>())
                {
                    foreach (var member2 in c.Members.OfType<FieldDeclarationSyntax>())
                    {
                        if (member1.SpanStart < member2.SpanStart)
                        {
                            pairs.Add((member1, member2));
                        }
                    }
                }

                return pairs;
            }
        }
    }
}

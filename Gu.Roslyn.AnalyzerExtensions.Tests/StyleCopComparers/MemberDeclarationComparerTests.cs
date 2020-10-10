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

    public static class MemberDeclarationComparerTests
    {
        private static readonly FieldInfo PositionField = typeof(SyntaxNode).GetField("<Position>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases(
            @"
namespace N
{
    public class C : IC
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
        
        public C()
        {
        }

        public event EventHandler PublicEvent1;

        public event EventHandler PublicEvent2
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        internal event EventHandler InternalEvent1;

        internal event EventHandler InternalEvent2
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public static int PublicStaticGet { get; } = 1;

        public static int PublicStaticGet1 { get; } = PublicStaticGet;

        public static int PublicStaticExpressionBody => PublicStaticGet;

        public int PublicGet { get; }

        object IC.PublicGet => this.PublicGet;

        public int PublicExpressionBody => this.PublicGet;

        public int PublicGetPrivateSet1 { get; private set; }

        public int PublicGetPrivateSet2 { get; private set; }

        public int PublicGetInternalSet { get; internal set; }

        public int PublicGetSet { get; set; }

        object IC.PublicGetSet
        {
            get { return this.PublicGetSet; }
            set { this.PublicGetSet = (int) value; }
        }

        internal int InternalGet { get; }

        internal int InternalExpressionBody => this.InternalGet;

        internal int InternalGetPrivateSet { get; private set; }

        internal int InternalGetSet { get; set; }

        private int PrivateGet { get; }

        private int PrivateExpressionBody => this.InternalGet;

        private int PrivateGetSet { get; set; }
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
        object PublicGet { get; }

        object PublicGetSet { get; set; }

        object Public();
    }
}",
            stripLines: false);

        private static readonly IReadOnlyList<TestCaseData> InitializedSource = CreateTestCases(
    @"
namespace N
{
    public class C
    {
        const int Const1 { get; } = 1;
        const int Const2 { get; } = Const1;

        private static int Static1 { get; } = 1;
        public static int PublicStatic2 { get; } = Static1;
        public static int Static3 { get; } = 2;
        public static int Static4 { get; } = Const1;
        public static int Static5 { get; } = Const2;
        public static int Static6 { get; } = Const1 + Const2;
    }
}",
    stripLines: true);

        [TestCaseSource(nameof(TestCaseSource))]
        public static void MemberDeclarationComparerCompare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(InitializedSource))]
        public static void InitializedWithOther(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1,  MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0,  MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0,  MemberDeclarationComparer.Compare(y, y));
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

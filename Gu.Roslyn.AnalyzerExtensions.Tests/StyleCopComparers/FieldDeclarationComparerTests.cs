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

    public static class FieldDeclarationComparerTests
    {
        private static readonly FieldInfo PositionField = typeof(SyntaxNode).GetField("<Position>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private static readonly IReadOnlyList<TestCaseData> ModifiersSource = CreateTestCases(
            @"
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
}",
            stripLines: false);

        private static readonly IReadOnlyList<TestCaseData> BackingFieldSource = CreateTestCases(
            @"
namespace N
{
    class C
    {
        private static int static1;
        private static int static2;

        private int value1;
        private int value2;
        private int value3;


        public static int Static1
        {
            get => static1;
            set => static1 = value;
        }

        public static int Static2
        {
            get => static2;
            set => static2 = value;
        }

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

        public int Value3
        {
            get => value3;
            set => value3 = value;
        }
    }
}",
            stripLines: true);

        private static readonly IReadOnlyList<TestCaseData> InitializedSource = CreateTestCases(
            @"
namespace N
{
    class C
    {
        public const int PublicConst1 = 1;
        public const int PublicConst2 = PublicConst1;
        public const int PublicConst3 = PublicConst2 * PublicConst1;
    }
}",
            stripLines: true);

        private static readonly IReadOnlyList<TestCaseData> DependencyPropertyBackingFieldSource = CreateTestCases(
            @"
namespace N
{
    using System.Windows;
    using System.Windows.Controls;

    public class C : Control
    {
        /// <summary>Identifies the <see cref=""Value1""/> dependency property.</summary>
        public static readonly DependencyProperty Value1Property = DependencyProperty.Register(
            nameof(Value1),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        /// <summary>Identifies the <see cref=""Value2""/> dependency property.</summary>
        public static readonly DependencyProperty Value2Property = DependencyProperty.Register(
            nameof(Value2),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        /// <summary>Identifies the <see cref=""Value3""/> dependency property.</summary>
        public static readonly DependencyProperty Value3Property = DependencyProperty.Register(
            nameof(Value3),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        public int Value1
        {
            get => (int)this.GetValue(Value1Property);
            set => this.SetValue(Value1Property, value);
        }

        public int Value2
        {
            get => (int)this.GetValue(Value2Property);
            set => this.SetValue(Value2Property, value);
        }

        public int Value3
        {
            get => (int)this.GetValue(Value3Property);
            set => this.SetValue(Value3Property, value);
        }
    }
}",
            stripLines: true);

        [TestCaseSource(nameof(ModifiersSource))]
        public static void Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(ModifiersSource))]
        public static void MemberDeclarationComparerCompare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(InitializedSource))]
        public static void InitializedWithOther(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(BackingFieldSource))]
        public static void BackingField(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(DependencyPropertyBackingFieldSource))]
        public static void DependencyPropertyBackingField(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, FieldDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, FieldDeclarationComparer.Compare(y, y));
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

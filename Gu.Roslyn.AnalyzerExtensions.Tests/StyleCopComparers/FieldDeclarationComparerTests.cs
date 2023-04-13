namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparers;

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
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    class C
    {
        private static int static1;
        private static int static2;
        private static int static3;
        private static int static4;

        private int value1;
        private int value2;
        private int value3;
        private int value4;
        private int value5;
        private int value6;

        public static int Static1
        {
            get => static1;
            set => static1 = value;
        }

        public static int Static2
        {
            get
            {
                return static2;
            }

            set
            {
                static2 = value;
            }
        }

        public static int Static3
        {
            get => static3;
            set => static3 = value;
        }

        public static int Static4
        {
            get
            {
                return static4;
            }

            set
            {
                static4 = value;
            }
        }

        public int Value1
        {
            get => value1;
            set => value1 = value;
        }

        public int Value2
        {
            get
            {
                return this.value2;
            }

            set
            {
                this.value2 = value;
            }
        }

        public int Value3
        {
            get => value3;
            set => TrySet(ref value3, value);
        }

        public int Value4
        {
            get
            {
                return this.value4;
            }

            set
            {
                this.value4 = value;
            }
        }

        public int Value5
        {
            get => this.value5;
            set => this.TrySet(ref this.value5, value);
        }

        public int Value6
        {
            get => this.value6;
            set => this.value6 = value;
        }

        private bool TrySet<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                return true;
            }

            field = value;
            return false;
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

    private static readonly IReadOnlyList<TestCaseData> DependencyPropertySource = CreateTestCases(
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

        private static readonly DependencyPropertyKey ReadOnly1PropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ReadOnly1),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        /// <summary>Identifies the <see cref=""ReadOnly1""/> dependency property.</summary>
        public static readonly DependencyProperty ReadOnly1Property = ReadOnly1PropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref=""Value2""/> dependency property.</summary>
        public static readonly DependencyProperty Value2Property = DependencyProperty.Register(
            nameof(Value2),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        private static readonly DependencyPropertyKey ReadOnly2PropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ReadOnly2),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        /// <summary>Identifies the <see cref=""ReadOnly2""/> dependency property.</summary>
        public static readonly DependencyProperty ReadOnly2Property = ReadOnly2PropertyKey.DependencyProperty;

        /// <summary>Identifies the <see cref=""Value3""/> dependency property.</summary>
        public static readonly DependencyProperty Value3Property = DependencyProperty.Register(
            nameof(Value3),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        private static readonly DependencyPropertyKey ReadOnly3PropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ReadOnly3),
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int)));

        /// <summary>Identifies the <see cref=""ReadOnly3""/> dependency property.</summary>
        public static readonly DependencyProperty ReadOnly3Property = ReadOnly3PropertyKey.DependencyProperty;

        public static readonly DependencyProperty Attached1Property = DependencyProperty.RegisterAttached(
            ""Attached1"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int));

        public static readonly DependencyProperty Attached2Property = DependencyProperty.RegisterAttached(
            ""Attached2"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int));

        public static readonly DependencyProperty Attached3Property = DependencyProperty.RegisterAttached(
            ""Attached3"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(default(int));

        public int Value1
        {
            get => (int)this.GetValue(Value1Property);
            set => this.SetValue(Value1Property, value);
        }

        public int ReadOnly1
        {
            get => (int)this.GetValue(ReadOnly1Property);
            private set => this.SetValue(ReadOnly1PropertyKey, value);
        }

        public int Value2
        {
            get => (int)this.GetValue(Value2Property);
            set => this.SetValue(Value2Property, value);
        }

        public int ReadOnly2
        {
            get => (int)this.GetValue(ReadOnly2Property);
            private set => this.SetValue(ReadOnly2PropertyKey, value);
        }

        public int Value3
        {
            get => (int)this.GetValue(Value3Property);
            set => this.SetValue(Value3Property, value);
        }

        public int ReadOnly3
        {
            get => (int)this.GetValue(ReadOnly3Property);
            private set => this.SetValue(ReadOnly3PropertyKey, value);
        }

        public static int GetAttached1(DependencyObject element)
        {
            return (int)element.GetValue(Attached1Property);
        }

        public static void SetAttached1(DependencyObject element, int value)
        {
            element.SetValue(Attached1Property, value);
        }

        public static int GetAttached2(DependencyObject element)
        {
            return (int)element.GetValue(Attached2Property);
        }

        public static void SetAttached2(DependencyObject element, int value)
        {
            element.SetValue(Attached2Property, value);
        }

        public static int GetAttached3(DependencyObject element)
        {
            return (int)element.GetValue(Attached3Property);
        }

        public static void SetAttached3(DependencyObject element, int value)
        {
            element.SetValue(Attached3Property, value);
        }
    }
}",
        stripLines: true);

    private static readonly IReadOnlyList<TestCaseData> AttachedPropertySource = CreateTestCases(
        @"
namespace N
{
    using System;
    using System.Windows;

    public static class C
    {
        public static readonly DependencyProperty Value1Property = DependencyProperty.RegisterAttached(
            ""Value1"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(
                default(int),
                OnValue1Changed,
                OnValue1Coerce),
            ValidateValue1);

        private static readonly DependencyPropertyKey Value2PropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            ""Value2"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(
                default(int),
                OnValue2Changed,
                OnValue2Coerce),
            ValidateValue2);

        public static readonly DependencyProperty Value2Property = Value2PropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey Value3PropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            ""Value3"",
            typeof(int),
            typeof(C),
            new PropertyMetadata(
                default(int),
                OnValue3Changed,
                OnValue3Coerce),
            ValidateValue3);

        public static readonly DependencyProperty Value3Property = Value3PropertyKey.DependencyProperty;

        public static int GetValue1(DependencyObject element)
        {
            return (int)element.GetValue(Value1Property);
        }

        public static void SetValue1(DependencyObject element, int value)
        {
            element.SetValue(Value1Property, value);
        }

        public static int GetValue2(DependencyObject element)
        {
            return (int)element.GetValue(Value2Property);
        }

        private static void SetValue2(DependencyObject element, int value)
        {
            element.SetValue(Value2PropertyKey, value);
        }

        public static int GetValue3(DependencyObject element)
        {
            return (int)element.GetValue(Value3Property);
        }

        private static void SetValue3(DependencyObject element, int value)
        {
            element.SetValue(Value3PropertyKey, value);
        }

        private static void OnValue1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object OnValue1Coerce(DependencyObject d, object basevalue)
        {
            throw new NotImplementedException();
        }

        private static bool ValidateValue1(object value)
        {
            throw new NotImplementedException();
        }

        private static void OnValue2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object OnValue2Coerce(DependencyObject d, object basevalue)
        {
            throw new NotImplementedException();
        }

        private static bool ValidateValue2(object value)
        {
            throw new NotImplementedException();
        }

        private static void OnValue3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object OnValue3Coerce(DependencyObject d, object basevalue)
        {
            throw new NotImplementedException();
        }

        private static bool ValidateValue3(object value)
        {
            throw new NotImplementedException();
        }
    }
}
",
        stripLines: true);

    [TestCaseSource(nameof(ModifiersSource))]
    public static void Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
    }

    [TestCaseSource(nameof(ModifiersSource))]
    public static void MemberDeclarationComparerCompare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  MemberDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(y, y));
    }

    [TestCaseSource(nameof(InitializedSource))]
    public static void InitializedWithOther(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
        Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  MemberDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(y, y));
    }

    [TestCaseSource(nameof(BackingFieldSource))]
    public static void BackingField(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
        Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  MemberDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(y, y));
    }

    [TestCaseSource(nameof(DependencyPropertySource))]
    public static void DependencyProperty(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
        Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  MemberDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  MemberDeclarationComparer.Compare(y, y));
    }

    [TestCaseSource(nameof(AttachedPropertySource))]
    public static void AttachedProperty(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
    {
        Assert.AreEqual(-1, FieldDeclarationComparer.Compare(x, y));
        Assert.AreEqual(1,  FieldDeclarationComparer.Compare(y, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(x, x));
        Assert.AreEqual(0,  FieldDeclarationComparer.Compare(y, y));
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

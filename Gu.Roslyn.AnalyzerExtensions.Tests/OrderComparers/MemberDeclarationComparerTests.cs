namespace Gu.Roslyn.AnalyzerExtensions.Tests.OrderComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class MemberDeclarationComparerTests
    {
        private static readonly SyntaxTree SyntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo : IFoo
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

        object IFoo.PublicGet => this.PublicGet;

        public int PublicExpressionBody => this.PublicGet;

        public int PublicGetPrivateSet1 { get; private set; }

        public int PublicGetPrivateSet2 { get; private set; }

        public int PublicGetInternalSet { get; internal set; }

        public int PublicGetSet { get; set; }

        object IFoo.PublicGetSet
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

        object IFoo.Public() => 1;

        internal static int InternalStatic() => 1;

        internal int Internal() => 1;

        private static int PrivateStatic() => 1;

        private int Private() => 1;
    }

    public interface IFoo
    {
        object PublicGet { get; }

        object PublicGetSet { get; set; }

        object Public();
    }
}");
        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases().ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public void MemberDeclarationComparerCompare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
        }

        [Test]
        public void InitializedWithOther()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public static int PublicStatic1 { get; } = PublicStatic2;
        public static int PublicStatic2 { get; } = 3;
    }
}");
            var x = syntaxTree.FindPropertyDeclaration("public static int PublicStatic1 { get; } = PublicStatic2");
            var y = syntaxTree.FindPropertyDeclaration("public static int PublicStatic2 { get; } = 3");
            Assert.AreEqual(1, MemberDeclarationComparer.Compare(x, y));
            Assert.AreEqual(-1, MemberDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, MemberDeclarationComparer.Compare(y, y));
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

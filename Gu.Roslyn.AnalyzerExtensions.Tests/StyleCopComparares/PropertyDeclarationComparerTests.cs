namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparares
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class PropertyDeclarationComparerTests
    {
        private static readonly SyntaxTree SyntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo : IFoo
    {
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
    }

    public interface IFoo
    {
        object PublicGet { get; }

        object PublicGetSet { get; set; }
    }
}");

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases().ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public void Compare(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            Assert.AreEqual(-1, PropertyDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, PropertyDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, PropertyDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, PropertyDeclarationComparer.Compare(y, y));
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public void MemberDeclarationComparerCompare(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
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
            Assert.AreEqual(1, PropertyDeclarationComparer.Compare(x, y));
            Assert.AreEqual(-1, PropertyDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, PropertyDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, PropertyDeclarationComparer.Compare(y, y));
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

namespace Gu.Roslyn.AnalyzerExtensions.Tests.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class BasePropertyDeclarationComparerTests
    {
        private static readonly SyntaxTree SyntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;

    public class Foo : IFoo
    {
        public event EventHandler PublicEvent1
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public event EventHandler PublicEvent2
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        internal event EventHandler InternalEvent2
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public static int PublicStaticGet { get; } = 1;

        public static int PublicStaticGet1 { get; } = PublicStaticGet;

        public static int PublicStaticExpressionBody => PublicStaticGet;

        public static int PublicStaticGetSet { get; set; }

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
            set { this.PublicGetSet = (int)value; }
        }

        internal static int InternalStaticGet { get; } = 1;

        internal static int InternalStaticGet1 { get; } = InternalStaticGet;

        internal static int InternalStaticExpressionBody => InternalStaticGet;

        internal static int InternalStaticGetSet { get; set; }

        internal int InternalGet { get; }

        internal int InternalExpressionBody => this.InternalGet;

        internal int InternalGetPrivateSet { get; private set; }

        internal int InternalGetSet { get; set; }

        private int PrivateGet { get; }

        private int PrivateExpressionBody => this.InternalGet;

        private int PrivateGetSet { get; set; }

        public int this[int index] => index;
    }

    public interface IFoo
    {
        object PublicGet { get; }

        object PublicGetSet { get; set; }
    }
}");

        private static readonly IReadOnlyList<TestCaseData> TestCaseSource = CreateTestCases().ToArray();

        [TestCaseSource(nameof(TestCaseSource))]
        public void Compare(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y)
        {
            Assert.AreEqual(-1, BasePropertyDeclarationComparer.Compare(x, y));
            Assert.AreEqual(1, BasePropertyDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, BasePropertyDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, BasePropertyDeclarationComparer.Compare(y, y));
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
            Assert.AreEqual(1, BasePropertyDeclarationComparer.Compare(x, y));
            Assert.AreEqual(-1, BasePropertyDeclarationComparer.Compare(y, x));
            Assert.AreEqual(0, BasePropertyDeclarationComparer.Compare(x, x));
            Assert.AreEqual(0, BasePropertyDeclarationComparer.Compare(y, y));
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

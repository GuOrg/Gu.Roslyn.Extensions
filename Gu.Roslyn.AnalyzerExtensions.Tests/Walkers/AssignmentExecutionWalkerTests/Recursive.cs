namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers.AssignmentExecutionWalkerTests
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class Recursive
    {
        [Test]
        public static void Properties()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C1
    {
        private C bar1;
        private C bar2;

        public C2 P1
        {
            get
            {
                return this.bar1;
            }

            set
            {
                if (Equals(value, this.bar1))
                {
                    return;
                }

                if (value != null && this.bar2 != null)
                {
                    this.P2 = null;
                }

                if (this.bar1 != null)
                {
                    this.bar1.Selected = false;
                }

                this.bar1 = value;
                if (this.bar1 != null)
                {
                    this.bar1.Selected = true;
                }
            }
        }

        public C2 P2
        {
            get
            {
                return this.bar2;
            }

            set
            {
                if (Equals(value, this.bar2))
                {
                    return;
                }

                if (value != null && this.bar1 != null)
                {
                    this.P1 = null;
                }

                if (this.bar2 != null)
                {
                    this.bar2.Selected = false;
                }

                this.bar2 = value;
                if (this.bar2 != null)
                {
                    this.bar2.Selected = true;
                }
            }
        }
    }

    public class C2
    {
        public bool Selected { get; set; }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var setter = syntaxTree.FindPropertyDeclaration("P2").Find<AccessorDeclarationSyntax>("set");
            using var assignedValues = AssignmentExecutionWalker.Borrow(setter, SearchScope.Recursive, semanticModel, CancellationToken.None);
            var actual = string.Join(", ", assignedValues.Assignments);
            var expected = "this.P2 = null, this.P1 = null, this.P2 = null, this.bar1.Selected = false, this.bar1 = value, this.bar1.Selected = true, this.bar2.Selected = false, this.bar2 = value, this.bar2.Selected = true, this.bar1.Selected = false, this.bar1 = value, this.bar1.Selected = true";
            Assert.AreEqual(expected, actual);
        }
    }
}

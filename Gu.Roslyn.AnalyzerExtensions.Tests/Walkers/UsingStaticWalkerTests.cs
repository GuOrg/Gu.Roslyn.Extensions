namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class UsingStaticWalkerTests
    {
        [Test]
        public void Borrow()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using static NUnit.Framework.Assert;

    public class C
    {
        public C()
        {
            AreEqual(1, 1);
        }
    }
}");
            using (var walker = UsingStaticWalker.Borrow(tree))
            {
                CollectionAssert.AreEqual(new[] { "using static NUnit.Framework.Assert;" }, walker.UsingDirectives.Select(x => x.ToString()));
            }

            using (var walker = UsingStaticWalker.Borrow(tree.GetRoot(CancellationToken.None)))
            {
                CollectionAssert.AreEqual(new[] { "using static NUnit.Framework.Assert;" }, walker.UsingDirectives.Select(x => x.ToString()));
            }
        }
    }
}

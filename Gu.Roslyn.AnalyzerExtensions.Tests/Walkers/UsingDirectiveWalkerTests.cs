namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers;

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class UsingDirectiveWalkerTests
{
    [Test]
    public static void Borrow()
    {
        var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System;
    using static NUnit.Framework.Assert;

    public class C
    {
        public C()
        {
            AreEqual(1, 1);
        }
    }
}");
        var expected = new[] { "using System;", "using static NUnit.Framework.Assert;" };
        using (var walker = UsingDirectiveWalker.Borrow(tree))
        {
            CollectionAssert.AreEqual(expected, walker.UsingDirectives.Select(x => x.ToString()));
        }

        using (var walker = UsingDirectiveWalker.Borrow(tree.GetRoot(CancellationToken.None)))
        {
            CollectionAssert.AreEqual(expected, walker.UsingDirectives.Select(x => x.ToString()));
        }
    }
}

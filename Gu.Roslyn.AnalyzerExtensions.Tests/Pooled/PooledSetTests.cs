// ReSharper disable UnusedVariable
#pragma warning disable IDE0059 // Unnecessary assignment of a value
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Pools;

using NUnit.Framework;

public static class PooledSetTests
{
    [Test]
    public static void UsingBorrow()
    {
        using (PooledSet<int>.Borrow())
        {
        }
    }

    [Test]
    public static void UsingBorrowBorrowOrIncrementUsage()
    {
        using var set = PooledSet<int>.Borrow();
        using var meh = set.IncrementUsage();
        using var meh1 = meh.IncrementUsage();
    }

    [TestCase(null)]
    public static void UsingBorrowOrIncrementUsageNull(PooledSet<int> arg)
    {
        using var set = arg.IncrementUsage();
        using var meh = set.IncrementUsage();
        using var meh1 = meh.IncrementUsage();
    }

    [Test]
    public static void UseSet()
    {
        using var set = ((PooledSet<int>)null).IncrementUsage();
        UseSet(set);
        UseSet(set);
    }

    [Test]
    public static void UsingBorrowAddForeach()
    {
        using var set = PooledSet<int>.Borrow();
        _ = set.Add(1);
        foreach (var i in set)
        {
        }
    }

    [Test]
    public static void UsingBorrowAddForeachCallId()
    {
        using var set = PooledSet<int>.Borrow();
        _ = set.Add(1);
        foreach (var i in set)
        {
            var j = Id(i);
        }
    }

    public static int Id(int n) => n;

    public static void UseSet(PooledSet<int> set)
    {
        using (set = set.IncrementUsage())
        {
            _ = set.Add(set.Count);
            foreach (var i in set)
            {
                var j = Id(i);
            }
        }
    }
}

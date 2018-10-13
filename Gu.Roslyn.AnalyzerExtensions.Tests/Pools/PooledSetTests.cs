// ReSharper disable UnusedVariable
namespace Gu.Roslyn.AnalyzerExtensions.Tests.Pools
{
    using NUnit.Framework;

    public class PooledSetTests
    {
        [Test]
        public void UsingBorrow()
        {
            using (PooledSet<int>.Borrow())
            {
            }
        }

        [Test]
        public void UsingBorrowBorrowOrIncrementUsage()
        {
            using (var set = PooledSet<int>.Borrow())
            {
                using (var meh = set.IncrementUsage())
                {
                    using (var meh1 = meh.IncrementUsage())
                    {
                    }
                }
            }
        }

        [TestCase(null)]
        public void UsingBorrowOrIncrementUsageNull(PooledSet<int> arg)
        {
            using (var set = arg.IncrementUsage())
            {
                using (var meh = set.IncrementUsage())
                {
                    using (var meh1 = meh.IncrementUsage())
                    {
                    }
                }
            }
        }

        [Test]
        public void UseSet()
        {
            using (var set = ((PooledSet<int>)null).IncrementUsage())
            {
                UseSet(set);
                UseSet(set);
            }
        }

        [Test]
        public void UsingBorrowAddForeach()
        {
            using (var set = PooledSet<int>.Borrow())
            {
                set.Add(1);
                foreach (var i in set)
                {
                }
            }
        }

        [Test]
        public void UsingBorrowAddForeachCallId()
        {
            using (var set = PooledSet<int>.Borrow())
            {
                set.Add(1);
                foreach (var i in set)
                {
                    var j = Id(i);
                }
            }
        }

        private static int Id(int n) => n;

        private static void UseSet(PooledSet<int> set)
        {
            using (set = set.IncrementUsage())
            {
                set.Add(set.Count);
                foreach (var i in set)
                {
                    var j = Id(i);
                }
            }
        }
    }
}

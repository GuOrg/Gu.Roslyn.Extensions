namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    public partial class EnumerableExtTests
    {
        internal class WhenSourceIsEnumerable
        {
            [Test]
            public void TryFirst()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryFirst(out var result));
                Assert.AreEqual(1, result);
            }

            [Test]
            public void TryFirstWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryFirst(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryFirstWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryFirst(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryFirstWithPredicate()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryFirst(x => x == 2, out var result));
                Assert.AreEqual(2, result);
            }

            [Test]
            public void TryFirstWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryFirst(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryFirstWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryFirst(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingle()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 1).TrySingle(out var result));
                Assert.AreEqual(1, result);
            }

            [Test]
            public void TrySingleWhenMoreThanOne()
            {
                Assert.AreEqual(false, Enumerable.Range(0, 3).TrySingle(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TrySingle(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TrySingle(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleWithPredicate()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 5).TrySingle(x => x == 2, out var result));
                Assert.AreEqual(2, result);
            }

            [Test]
            public void TrySingleWithPredicateWhenMoreThanOne()
            {
                Assert.AreEqual(false, Enumerable.Repeat(2, 3).TrySingle(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TrySingle(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TrySingle(out var result));
                Assert.AreEqual(0, result);
            }

            [TestCase(0, 1)]
            [TestCase(1, 2)]
            [TestCase(2, 3)]
            public void TryElementAt(int index, int expected)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryElementAt(index, out var result));
                Assert.AreEqual(expected, result);
            }

            [TestCase(5)]
            public void TryElementAtWhenOutOfBounds(int index)
            {
                Assert.AreEqual(false, Enumerable.Range(1, 3).TryElementAt(index, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryElementAtWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryElementAt(0, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryElementAtWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryElementAt(0, out var result));
                Assert.AreEqual(0, result);
            }
        }
    }
}

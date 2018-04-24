namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    public partial class EnumerableExtTests
    {
        internal class WhenSourceIsEnumerable
        {
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

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TryFirstWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryFirst(x => x == n, out var result));
                Assert.AreEqual(n, result);
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
            public void TryFirstOfType()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryFirstOfType(out int result));
                Assert.AreEqual(1, result);
            }

            [Test]
            public void TryFirstOfTypeWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryFirstOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryFirstOfTypeWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryFirstOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TryFirstOfTypeWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryFirstOfType(x => x == n, out int result));
                Assert.AreEqual(n, result);
            }

            [Test]
            public void TryFirstOfTypeWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryFirstOfType(x => x == 2, out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryFirstOfTypeWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryFirstOfType(x => x == 2, out int result));
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

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TrySingleWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 5).TrySingle(x => x == n, out var result));
                Assert.AreEqual(n, result);
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

            [Test]
            public void TrySingleOfType()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 1).TrySingleOfType(out int result));
                Assert.AreEqual(1, result);
            }

            [Test]
            public void TrySingleOfTypeWhenMoreThanOne()
            {
                Assert.AreEqual(false, Enumerable.Range(0, 3).TrySingleOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleOfTypeWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TrySingleOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleOfTypeWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TrySingleOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TrySingleOfTypeWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 5).TrySingleOfType(x => x == n, out int result));
                Assert.AreEqual(n, result);
            }

            [Test]
            public void TrySingleOfTypeWithPredicateWhenMoreThanOne()
            {
                Assert.AreEqual(false, Enumerable.Repeat(2, 3).TrySingleOfType(x => x == 2, out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleOfTypeWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TrySingleOfType(x => x == 2, out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TrySingleOfTypeWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TrySingleOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLast()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryLast(out var result));
                Assert.AreEqual(3, result);
            }

            [Test]
            public void TryLastWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryLast(out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLastWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryLast(out var result));
                Assert.AreEqual(0, result);
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TryLastWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryLast(x => x == n, out var result));
                Assert.AreEqual(n, result);
            }

            [Test]
            public void TryLastWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryLast(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLastWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryLast(x => x == 2, out var result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLastOfType()
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryLastOfType(out int result));
                Assert.AreEqual(3, result);
            }

            [Test]
            public void TryLastOfTypeWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryLastOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLastOfTypeWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryLastOfType(out int result));
                Assert.AreEqual(0, result);
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void TryLastOfTypeWithPredicate(int n)
            {
                Assert.AreEqual(true, Enumerable.Range(1, 3).TryLastOfType(x => x == n, out int result));
                Assert.AreEqual(n, result);
            }

            [Test]
            public void TryLastOfTypeWithPredicateWhenEmpty()
            {
                Assert.AreEqual(false, Enumerable.Empty<int>().TryLastOfType(x => x == 2, out int result));
                Assert.AreEqual(0, result);
            }

            [Test]
            public void TryLastOfTypeWithPredicateWhenNull()
            {
                Assert.AreEqual(false, ((IEnumerable<int>)null).TryLastOfType(x => x == 2, out int result));
                Assert.AreEqual(0, result);
            }
        }
    }
}

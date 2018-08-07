namespace Gu.Roslyn.AnalyzerExtensions.Tests
{
    using NUnit.Framework;

    public class StringHelperTests
    {
        [TestCase("ab", "a", "b", true)]
        [TestCase("ab", "ab", "", true)]
        [TestCase("ab", "ab", null, true)]
        [TestCase("ab", null, "ab", true)]
        [TestCase("ab", "", "ab", true)]
        [TestCase("ab", "c", "b", false)]
        [TestCase("ab", "abc", "", false)]
        [TestCase("ab", "", null, false)]
        [TestCase("ab", null, null, false)]
        [TestCase("ab", "", "abc", false)]
        [TestCase("ab", null, "abc", false)]
        [TestCase("ab", "cd", "ab", false)]
        public void IsTwoParts(string text, string part1, string part2, bool expected)
        {
            Assert.AreEqual(expected, text.IsParts(part1, part2));
        }

        [TestCase("<summary>Identifies the <see cref=\"Bar\"/> dependency property.</summary>", "<summary>Identifies the <see cref=\"", "Bar", "\"/> dependency property.</summary>", true)]
        public void IsThreeParts(string text, string part1, string part2, string part3, bool expected)
        {
            Assert.AreEqual(expected, text.IsParts(part1, part2, part3));
        }

        [TestCase("abc", "a", "b", "c", null, true)]
        [TestCase("abc", "a", "b", null, "c", true)]
        [TestCase("abc", "a", null, "b", "c", true)]
        [TestCase("abc", null, "a", "b", "c", true)]
        [TestCase("abcd", "a", "b", "c", "d", true)]
        public void IsFourParts(string text, string part1, string part2, string part3, string part4, bool expected)
        {
            Assert.AreEqual(expected, text.IsParts(part1, part2, part3, part4));
        }
    }
}

namespace Gu.Roslyn.AnalyzerExtensions.Tests;

using NUnit.Framework;

public static class StringHelperTests
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
    public static void IsTwoParts(string text, string part1, string part2, bool expected)
    {
        Assert.AreEqual(expected, text.IsParts(part1, part2));
    }

    [TestCase("<summary>Identifies the <see cref=\"P\"/> dependency property.</summary>", "<summary>Identifies the <see cref=\"", "P", "\"/> dependency property.</summary>", true)]
    public static void IsThreeParts(string text, string part1, string part2, string part3, bool expected)
    {
        Assert.AreEqual(expected, text.IsParts(part1, part2, part3));
    }

    [TestCase("abc", "a", "b", "c", null, true)]
    [TestCase("abc", "a", "b", null, "c", true)]
    [TestCase("abc", "a", null, "b", "c", true)]
    [TestCase("abc", null, "a", "b", "c", true)]
    [TestCase("abcd", "a", "b", "c", "d", true)]
    public static void IsFourParts(string text, string part1, string part2, string part3, string part4, bool expected)
    {
        Assert.AreEqual(expected, text.IsParts(part1, part2, part3, part4));
    }

    [TestCase("abcd", "a", "b", "c", "d", null, true)]
    [TestCase("abcd", "a", "b", "c", null, "d", true)]
    [TestCase("abcd", "a", "b", null, "c", "d", true)]
    [TestCase("abcd", "a", null, "b", "c", "d", true)]
    [TestCase("abcd", null, "a", "b", "c", "d", true)]
    [TestCase("abcde", "a", "b", "c", "d", "e", true)]
    public static void IsFiveParts(string text, string part1, string part2, string part3, string part4, string part5, bool expected)
    {
        Assert.AreEqual(expected, text.IsParts(part1, part2, part3, part4, part5));
    }
}

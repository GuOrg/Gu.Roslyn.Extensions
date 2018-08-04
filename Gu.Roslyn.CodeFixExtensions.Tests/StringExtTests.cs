namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using NUnit.Framework;

    public class StringExtTests
    {
        [TestCase("abc", null, "abc")]
        [TestCase("abc\r\ncde", "  ", "  abc\r\n  cde")]
        [TestCase("abc", "    ", "    abc")]
        [TestCase("    abc", "    ", "    abc")]
        [TestCase("  abc", "    ", "    abc")]
        [TestCase("        abc", "    ", "    abc")]
        public void WithLeadingWhiteSpace(string text, string whitespace, string expected)
        {
            Assert.AreEqual(expected, text.WithLeadingWhiteSpace(whitespace));
        }
    }
}

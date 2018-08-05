namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using NUnit.Framework;

    public class StringExtTests
    {
        [TestCase("abc", null, "abc")]
        [TestCase("abc\r\n", "    ", "    abc\r\n")]
        [TestCase("    abc", "    ", "    abc")]
        [TestCase("  abc", "    ", "    abc")]
        [TestCase("        abc", "    ", "    abc")]
        [TestCase("abc\r\ncde", "  ", "  abc\r\n  cde")]
        [TestCase("  abc\r\ncde", "  ", "  abc\r\n  cde")]
        [TestCase("  abc\r\n  cde", "  ", "  abc\r\n  cde")]
        [TestCase("    abc\r\n    cde", "  ", "  abc\r\n  cde")]
        [TestCase("abc\r\ncde\r\n", "  ", "  abc\r\n  cde\r\n")]
        public void WithLeadingWhiteSpace(string text, string whitespace, string expected)
        {
            Assert.AreEqual(expected, text.WithLeadingWhiteSpace(whitespace));
        }

        [TestCase("abc", "Abc")]
        [TestCase("Abc", "Abc")]
        public void ToFirstCharUpper(string text, string expected)
        {
            Assert.AreEqual(expected, text.ToFirstCharUpper());
        }

        [TestCase("abc", "abc")]
        [TestCase("Abc", "abc")]
        public void ToFirstCharLower(string text, string expected)
        {
            Assert.AreEqual(expected, text.ToFirstCharLower());
        }
    }
}

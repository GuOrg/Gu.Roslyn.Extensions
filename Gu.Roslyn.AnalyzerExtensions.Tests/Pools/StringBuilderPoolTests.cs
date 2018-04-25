namespace Gu.Roslyn.AnalyzerExtensions.Tests.Pools
{
    using NUnit.Framework;

    public class StringBuilderPoolTests
    {
        [Test]
        public void BorrowAppendLineReturn()
        {
            var text = StringBuilderPool.Borrow()
                                        .AppendLine("a")
                                        .AppendLine()
                                        .AppendLine("b")
                                        .Return();
            Assert.AreEqual("a\r\n\r\nb\r\n", text);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;

namespace Emme.Test.Models
{
    /// <summary>
    /// Tests of Emme.Models.Span.
    /// </summary>
    [TestClass]
    public class SpanTest
    {
        /// <summary>
        /// Test Length property of an empty Span.
        /// </summary>
        [TestMethod]
        public void SpanLengthEmpty()
        {
            var span = new Span(1, 1);
            Assert.AreEqual(0, span.Length);
        }

        /// <summary>
        /// Test Lenth property of a non-empty Span.
        /// </summary>
        [TestMethod]
        public void SpanLengthNonEmpty()
        {
            var span = new Span(1, 3);
            Assert.AreEqual(2, span.Length);
        }
    }
}

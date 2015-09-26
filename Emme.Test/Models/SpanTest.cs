using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;

namespace Emme.Test
{
    [TestClass]
    public class SpanTest
    {
        [TestMethod]
        public void SpanCountEmpty()
        {
            var span = new Span(1, 1);
            Assert.AreEqual(0, span.Count);
        }

        [TestMethod]
        public void SpanCountNonEmpty()
        {
            var span = new Span(1, 3);
            Assert.AreEqual(2, span.Count);
        }
    }
}

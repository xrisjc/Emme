using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;

namespace Emme.Test.Models
{
    [TestClass]
    public class LineMarkerTests
    {
        /// <summary>
        /// Tests buffer index of an empty LineMarkers at the position (0, 0).
        /// </summary>
        [TestMethod]
        public void EmptyBufferIndex()
        {
            var lineMarkers = new LineMarkers();
            Assert.AreEqual(0, lineMarkers.BufferIndex(new Position(0, 0)));
        }

        /// <summary>
        /// Test the buffer index after an insertion at (0, 0) of position
        /// (0, 1).
        /// </summary>
        [TestMethod]
        public void OneInsertBufferIndex()
        {
            var lineMarkers = new LineMarkers();
            lineMarkers.Insert(new Position(0, 0));
            Assert.AreEqual(1, lineMarkers.BufferIndex(new Position(0, 1)));
        }
    }
}
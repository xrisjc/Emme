using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;

namespace Emme.Test.Models
{
    /// <summary>
    /// Tests of Emme.Models.GapBuffer.
    /// </summary>
    [TestClass]
    public class GapBufferTest
    {
        /// <summary>
        /// Tests the Count of a newly created GapBuffer.
        /// </summary>
        [TestMethod]
        public void NewBufferCount()
        {
            var gb = new GapBuffer<char>(initialCapacity: 1024);
            Assert.AreEqual(0, gb.Count);
        }

        /// <summary>
        /// Test the Count of a newly created GapBuffer with content inserted.
        /// </summary>
        [TestMethod]
        public void NewBufferInsertCount()
        {
            var gb = new GapBuffer<char>();
            gb.InsertAt(0, 'a');
            Assert.AreEqual(1, gb.Count);
        }

        /// <summary>
        /// Test that a value was inserted into a newly created GapBuffer.
        /// </summary>
        [TestMethod]
        public void NewBufferInsertValue()
        {
            var gb = new GapBuffer<char>();
            gb.InsertAt(0, 'a');
            Assert.AreEqual('a', gb[0]);
        }

        /// <summary>
        /// Test inserting a value in the middle of content such that the gap
        /// will have to move to the left.
        /// </summary>
        [TestMethod]
        public void InsertLeftOfGap()
        {
            var gb = new GapBuffer<char>();
            gb.InsertAt(0, 'a');
            gb.InsertAt(1, 'c');

            gb.InsertAt(1, 'b');

            Assert.AreEqual("abc", string.Join("", gb[0], gb[1], gb[2]));
        }

        /// <summary>
        /// Test inserting a value in the middle of content such that the gap
        /// will have to move to the right.
        /// </summary>
        [TestMethod]
        public void InsertRightOfGap()
        {
            var gb = new GapBuffer<char>();
            gb.InsertAt(0, 'b'); // "b^"
            gb.InsertAt(1, 'd'); // "bd^"
            gb.InsertAt(0, 'a'); // "a^bd"

            gb.InsertAt(2, 'c'); // "abc^d"

            Assert.AreEqual(
                "abcd",
                string.Join("", gb[0], gb[1], gb[2], gb[3]));
        }

        /// <summary>
        /// Test insertion on filled buffer.
        /// </summary>
        [TestMethod]
        public void InsertGrowBuffer()
        {
            var gb = new GapBuffer<char>(2);
            gb.InsertAt(0, 'a');
            gb.InsertAt(1, 'c');

            gb.InsertAt(1, 'b');

            Assert.AreEqual("abc", string.Join("", gb[0], gb[1], gb[2]));
        }

        /// <summary>
        /// Test deletion method.
        /// </summary>
        [TestMethod]
        public void Delete()
        {
            var gb = new GapBuffer<char>();
            gb.InsertAt(0, 'a');
            gb.InsertAt(1, 'c');
            gb.InsertAt(1, 'b');

            gb.DeleteAt(0);

            Assert.AreEqual("bc", string.Join("", gb[0], gb[1]));
        }
    }
}

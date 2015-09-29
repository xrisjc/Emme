using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;

namespace Emme.Test.Models
{
    /// <summary>
    /// Tests of Emme.Models.Marker.
    /// </summary>
    [TestClass]
    public class MarkerTest
    {
        /// <summary>
        /// Test Length property of an empty Marker.
        /// </summary>
        [TestMethod]
        public void MarkerLengthEmpty()
        {
            var marker = new Marker(1, 1);
            Assert.AreEqual(0, marker.Length);
        }

        /// <summary>
        /// Test Lenth property of a non-empty Marker.
        /// </summary>
        [TestMethod]
        public void MarkerLengthNonEmpty()
        {
            var marker = new Marker(1, 3);
            Assert.AreEqual(2, marker.Length);
        }
    }
}

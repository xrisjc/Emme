using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emme.Models;
using System.IO;

namespace Emme.Test.Models
{
    [TestClass]
    public class DocumentTest
    {
        [TestMethod]
        public void EmptyBufferInsertOnFirstLine()
        {
            const string text = "Hello";
            var position = new Position(0, 0);
            var document = new Document();
            foreach (char value in text)
            {
                document.Insert(position, value);
                position = new Position(position.Line, position.Column + 1);
            }
            string documentText = null;
            using (var writer = new StringWriter())
            {
                document.Write(writer);
                documentText = writer.ToString();
            }
            Assert.AreEqual(text, documentText);
        }
    }
}
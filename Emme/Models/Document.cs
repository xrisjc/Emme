using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Models
{
    public class Document
    {
        private GapBuffer<char> buffer = new GapBuffer<char>();

        public void Insert(Position position, char value)
        {
            buffer.InsertAt(position.Column, value);
        }

        public void Write(TextWriter writer)
        {
            for (int i = 0; i < buffer.Count; i++)
            {
                writer.Write(buffer[i]);
            }
        }
    }
}
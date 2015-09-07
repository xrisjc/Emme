//
// Copyright (C) 2010 - 2015  Christopher Cowan
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using Emme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Editing
{
  public class TextViewFactoryFile : ITextViewFactory
  {
    public string InitialContent { get; }

    public TextViewFactoryFile(string initialContent)
    {
      InitialContent = initialContent;
    }

    public TextView Create()
    {
      var content = new GapBuffer<char>(initialCapacity: InitialContent.Length);
      var lines = new GapBuffer<Span>();

      IIndexable<char> initialContentIndexable = new StringIndexable(InitialContent);
      string newLine = Environment.NewLine;
      int lineStartIndex = 0;
      int bufferIndex = 0;
      int line = 0;
      int newLineIndex;
      do
      {
        newLineIndex = InitialContent.IndexOf(newLine, startIndex: lineStartIndex);
        var lineSlice = new Span(lineStartIndex, (newLineIndex >= 0) ? newLineIndex : InitialContent.Length);
        content.Insert(bufferIndex, initialContentIndexable, lineSlice);
        lines.Insert(line, new Span(bufferIndex, bufferIndex + lineSlice.Length));
        lineStartIndex += lineSlice.Length + newLine.Length;
        bufferIndex += lineSlice.Length;
        line++;
      }
      while (newLineIndex >= 0);

      return new TextView(content, lines);
    }
  }
}

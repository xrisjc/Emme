//
// File: TextView.cs
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

using System;
using System.Collections.Generic;
using System.Text;
using Emme.Models;
using static System.Math;
using static Emme.Models.Util;

namespace Emme.Editing
{
  public class TextView
  {
    public GapBuffer<char> GapBuffer { get; }
    public GapBuffer<Span> Lines { get; }
    public Position Caret { get; set; } = Position.BufferStart;
    public ScrollView ScrollView { get; set; } = new ScrollView(lineStart: 0, columnStart: 0, lines: 24, columns: 80);
    public int? DesiredColumn { get; set; } = null;

    /// <summary>
    /// Primary constructor
    /// </summary>
    public TextView(GapBuffer<char> gapBuffer, GapBuffer<Span> lines)
    {
      GapBuffer = gapBuffer;
      Lines = lines;
    }

    /// <summary>
    /// The number of lines this TextView contains.
    /// </summary>
    public int LastLine => Lines.Count - 1;

    /// <summary>
    /// The index of the caret in the text buffer.
    /// </summary>
    public int CaretBufferIndex
      => Lines[Caret.Line].Start + Caret.Column;

    public void ResizeScrollView(int lines, int columns)
    {
      ScrollView.Resize(lines, columns);
    }

    public void GapBufferInsert(char valueToInsert)
    {
      GapBuffer.Insert(CaretBufferIndex, valueToInsert);
      ShiftLines(1);
    }

    public char GapBufferDelete()
    {
      char deletedValue = GapBuffer[CaretBufferIndex];
      GapBuffer.Delete(CaretBufferIndex);
      ShiftLines(-1);
      return deletedValue;
    }

    private void ShiftLines(int delta)
    {
      int start = Caret.Line;
      Lines[start] = Lines[start].MoveEnd(delta);
      for (int i = start + 1; i < Lines.Count; i++)
      {
        Lines[i] = Lines[i].Move(delta);
      }
    }

    public void MoveCaretToLine(int line)
    {
      if (line.IsInRange(0, Lines.Count))
      {
        DesiredColumn = DesiredColumn ?? Caret.Column;
        int nextColumn = Math.Min(Lines[line].Length, DesiredColumn.Value);
        Caret = new Position(line, nextColumn);
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      for (var line = 0; line < Lines.Count; line++)
      {
        for (int i = Lines[line].Start; i < Lines[line].End; i++)
        {
          sb.Append(GapBuffer[i]);
        }
        if (line + 1 < Lines.Count)
        {
          sb.Append(Environment.NewLine);
        }
      }
      return sb.ToString();
    }

    public IEnumerable<string> EnumerateLines()
    {
      var sb = new StringBuilder();
      int lineStart = ScrollView.LineStart;
      int lineEnd = Min(lineStart + ScrollView.Lines, Lines.Count);
      for (var line = lineStart; line < lineEnd; line++)
      {
        sb.Clear();
        int iStart = Lines[line].Start + ScrollView.ColumnStart;
        int iEnd = Min(iStart + ScrollView.Columns, Lines[line].End);
        for (int i = iStart; i < iEnd; i++)
        {
          sb.Append(GapBuffer[i]);
        }
        yield return sb.ToString();
      }
    }
  }
}

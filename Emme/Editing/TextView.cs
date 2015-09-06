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
    public GapBuffer<Span> Lines { get; } = new GapBuffer<Span>();
    public Position Caret { get; set; } = Position.BufferStart;
    public ScrollView ScrollView { get; set; } = new ScrollView(lineStart: 0, columnStart: 0, lines: 24, columns: 80);
    public int? DesiredColumn { get; set; } = null;

    public TextView(string initialContent = null)
    {
      if (string.IsNullOrEmpty(initialContent))
      {
        GapBuffer = new GapBuffer<char>();
        Lines.Insert(0, new Span(0, 0));
      }
      else
      {
        GapBuffer = new GapBuffer<char>(initialCapacity: initialContent.Length);

        IIndexable<char> initialContentIndexable = new StringIndexable(initialContent);
        string newLine = Environment.NewLine;
        int lineStartIndex = 0;
        int bufferIndex = 0;
        int line = 0;
        int newLineIndex;
        do
        {
          newLineIndex = initialContent.IndexOf(newLine, startIndex: lineStartIndex);
          var lineSlice = new Span(lineStartIndex, (newLineIndex >= 0) ? newLineIndex : initialContent.Length);
          GapBuffer.Insert(bufferIndex, initialContentIndexable, lineSlice);
          Lines.Insert(line, new Span(bufferIndex, bufferIndex + lineSlice.Length));
          lineStartIndex += lineSlice.Length + newLine.Length;
          bufferIndex += lineSlice.Length;
          line++;
        }
        while (newLineIndex >= 0);
      }
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
      ScrollView.ResizedTo(lines, columns);
    }

    public void ShiftLines(int delta)
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

    public void LineUp()
    {
      MoveCaretToLine(Caret.PreviousLine);
      ScrollView.CheckLineUp(Caret);
    }

    public void PageUp()
    {
      MoveCaretToLine(Caret.Line - Min(Caret.Line, ScrollView.Lines));
      ScrollView.CheckPageUp(Caret);
    }

    public void LineDown()
    {
      MoveCaretToLine(Caret.NextLine);
      ScrollView.CheckLineDown(Caret);
    }

    /// <summary>
    /// Go down no more that maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Maximum number of lines to go down.</param>
    public void LineDown(int maxLines)
    {
      int deltaTextView = Min(LastLine - Caret.Line, maxLines);
      MoveCaretToLine(Caret.Line + deltaTextView);
      ScrollView.CheckPageDown(Caret);
    }

    public void PageDown()
    {
      LineDown(ScrollView.Lines);
    }

    public void LineStart()
    {
      DesiredColumn = null;
      Caret = new Position(Caret.Line, column: 0);
      ScrollView.CheckHorizontalScroll(Caret);
    }

    public void LineEnd()
    {
      DesiredColumn = null;
      Caret = new Position(Caret.Line, column: Lines[Caret.Line].Length);
      ScrollView.CheckLineDown(Caret)
                .CheckHorizontalScroll(Caret);
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

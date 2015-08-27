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
  class TextView
  {
    public GapBuffer<char> GapBuffer { get; }
    public GapBuffer<Span> Lines { get; } = new GapBuffer<Span>();
    public Position CaretPosition { get; set; } = Position.BufferStart;
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
    private int CaretBufferIndex =>
      Lines[CaretPosition.Line].Start + CaretPosition.Column;

    public void ResizeScrollView(int lines, int columns)
    {
      ScrollView = ScrollView.ResizedTo(lines, columns);
    }

    private void Shift(int delta)
    {
      Lines[CaretPosition.Line] = Lines[CaretPosition.Line].MoveEnd(delta);
      for (int i = CaretPosition.Line + 1; i < Lines.Count; i++)
      {
        Lines[i] = Lines[i].Move(delta);
      }
    }

    public void Insert(char value)
    {
      DesiredColumn = null;
      GapBuffer.Insert(CaretBufferIndex, value);
      Shift(1);
      CaretPosition += Position.OneColumn;
      ScrollView = ScrollView.CheckHorizontalScroll(CaretPosition);
    }

    public void InsertNewLine()
    {
      DesiredColumn = null;
      Tuple<Span, Span> splitSpans = Lines[CaretPosition.Line].Split(CaretBufferIndex);
      Lines[CaretPosition.Line] = splitSpans.Item1;
      CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      Lines.Insert(CaretPosition.Line, splitSpans.Item2);
      ScrollView = ScrollView.CheckLineDown(CaretPosition);
    }

    public void Delete()
    {
      if (CaretPosition.Column < Lines[CaretPosition.Line].Length)
      {
        GapBuffer.Delete(CaretBufferIndex);
        Shift(-1);
      }
      else if (CaretPosition.NextLine < Lines.Count)
      {
        Lines[CaretPosition.Line] =
          Lines[CaretPosition.Line].Join(Lines[CaretPosition.NextLine]);
        Lines.Delete(CaretPosition.NextLine);
      }
    }

    public void DeleteBackwards()
    {
      if (CaretPosition != Position.BufferStart)
      {
        CharLeft();
        Delete();
        ScrollView = ScrollView.CheckLineUp(CaretPosition)
                               .CheckHorizontalScroll(CaretPosition);
      }
    }

    public void CharLeft()
    {
      DesiredColumn = null;
      if (CaretPosition.Column > 0)
      {
        CaretPosition -= Position.OneColumn;
      }
      else if (CaretPosition.Line > 0)
      {
        CaretPosition = new Position(CaretPosition.PreviousLine, column: Lines[CaretPosition.PreviousLine].Length);
      }
      ScrollView = ScrollView.CheckLineUp(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void WordLeft()
    {
      DesiredColumn = null;
      if (CaretPosition.Column > 0)
      {
        int iStart = CaretBufferIndex;
        int iMin = Lines[CaretPosition.Line].Start;
        int i = iStart;
        while (i > iMin && char.IsWhiteSpace(GapBuffer[i - 1]))
        {
          i--;
        }
        while (i > iMin && !char.IsWhiteSpace(GapBuffer[i - 1]))
        {
          i--;
        }
        CaretPosition -= new Position(0, iStart - i);
      }
      else if (CaretPosition.PreviousLine >= 0)
      {
        CaretPosition = new Position(CaretPosition.PreviousLine, column: Lines[CaretPosition.PreviousLine].Length);
      }
      ScrollView = ScrollView.CheckLineUp(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    public void CharRight()
    {
      DesiredColumn = null;
      if (CaretPosition.Column < Lines[CaretPosition.Line].Length)
      {
        CaretPosition += Position.OneColumn;
      }
      else if (CaretPosition.NextLine < Lines.Count)
      {
        CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      }
      ScrollView = ScrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void WordRight()
    {
      DesiredColumn = null;
      if (CaretPosition.Column < Lines[CaretPosition.Line].Length)
      {
        // Caret is not at the end of the line.
        int iStart = CaretBufferIndex;
        int iMax = Lines[CaretPosition.Line].End;
        int i = iStart;
        while (i < iMax && !char.IsWhiteSpace(GapBuffer[i]))
        {
          i++;
        }
        while (i < iMax && char.IsWhiteSpace(GapBuffer[i]))
        {
          i++;
        }
        CaretPosition += new Position(0, i - iStart);
      }
      else if (CaretPosition.NextLine <= LastLine)
      {
        // At the end of the line, and it's not the last line.
        CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      }
      ScrollView = ScrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void MoveToLine(int line)
    {
      if (line.IsInRange(0, Lines.Count))
      {
        DesiredColumn = DesiredColumn ?? CaretPosition.Column;
        int nextColumn = Math.Min(Lines[line].Length, DesiredColumn.Value);
        CaretPosition = new Position(line, nextColumn);
      }
    }

    public void LineUp()
    {
      MoveToLine(CaretPosition.PreviousLine);
      ScrollView = ScrollView.CheckLineUp(CaretPosition);
    }

    /// <summary>
    /// Go up no more than maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Number of lines to go up.</param>
    public void LineUp(int maxLines)
    {
      int deltaTextView = Min(CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line - deltaTextView);
      ScrollView = ScrollView.CheckPageUp(CaretPosition);
    }

    public void PageUp()
    {
      LineUp(ScrollView.Lines);
    }

    public void LineDown()
    {
      MoveToLine(CaretPosition.NextLine);
      ScrollView = ScrollView.CheckLineDown(CaretPosition);
    }

    /// <summary>
    /// Go down no more that maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Maximum number of lines to go down.</param>
    public void LineDown(int maxLines)
    {
      int deltaTextView = Min(LastLine - CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line + deltaTextView);
      ScrollView = ScrollView.CheckPageDown(CaretPosition);
    }

    public void PageDown()
    {
      LineDown(ScrollView.Lines);
    }

    public void LineStart()
    {
      DesiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: 0);
      ScrollView = ScrollView.CheckHorizontalScroll(CaretPosition);
    }

    public void LineEnd()
    {
      DesiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: Lines[CaretPosition.Line].Length);
      ScrollView = ScrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
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

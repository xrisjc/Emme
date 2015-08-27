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
    readonly GapBuffer<char> gapBuffer;
    readonly GapBuffer<Span> lines = new GapBuffer<Span>();
    Position caretPosition = Position.BufferStart;
    ScrollView scrollView = new ScrollView(lineStart: 0, columnStart: 0, lines: 24, columns: 80);
    int? desiredColumn = null;

    public TextView(string initialContent = null)
    {
      if (string.IsNullOrEmpty(initialContent))
      {
        gapBuffer = new GapBuffer<char>();
        lines.Insert(0, new Span(0, 0));
      }
      else
      {
        gapBuffer = new GapBuffer<char>(initialCapacity: initialContent.Length);

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
          gapBuffer.Insert(bufferIndex, initialContentIndexable, lineSlice);
          lines.Insert(line, new Span(bufferIndex, bufferIndex + lineSlice.Length));
          lineStartIndex += lineSlice.Length + newLine.Length;
          bufferIndex += lineSlice.Length;
          line++;
        }
        while (newLineIndex >= 0);
      }
    }

    /// <summary>
    /// Current caret position in this TextView.
    /// </summary>
    public Position CaretPosition
    {
      get { return caretPosition; }
      set { caretPosition = value; }
    }

    public ScrollView ScrollView
    {
      get { return scrollView; }
    }

    /// <summary>
    /// The number of lines this TextView contains.
    /// </summary>
    public int LastLine => lines.Count - 1;

    /// <summary>
    /// The index of the caret in the text buffer.
    /// </summary>
    private int CaretBufferIndex =>
      lines[CaretPosition.Line].Start + CaretPosition.Column;

    public void ResizeScrollView(int lines, int columns)
    {
      scrollView = scrollView.ResizedTo(lines, columns);
    }

    private void Shift(int delta)
    {
      lines[CaretPosition.Line] = lines[CaretPosition.Line].MoveEnd(delta);
      for (int i = CaretPosition.Line + 1; i < lines.Count; i++)
      {
        lines[i] = lines[i].Move(delta);
      }
    }

    public void Insert(char value)
    {
      desiredColumn = null;
      gapBuffer.Insert(CaretBufferIndex, value);
      Shift(1);
      CaretPosition += Position.OneColumn;
      scrollView = scrollView.CheckHorizontalScroll(CaretPosition);
    }

    public void InsertNewLine()
    {
      desiredColumn = null;
      Tuple<Span, Span> splitSpans = lines[CaretPosition.Line].Split(CaretBufferIndex);
      lines[CaretPosition.Line] = splitSpans.Item1;
      CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      lines.Insert(CaretPosition.Line, splitSpans.Item2);
      scrollView = scrollView.CheckLineDown(CaretPosition);
    }

    public void Delete()
    {
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        gapBuffer.Delete(CaretBufferIndex);
        Shift(-1);
      }
      else if (CaretPosition.NextLine < lines.Count)
      {
        lines[CaretPosition.Line] =
          lines[CaretPosition.Line].Join(lines[CaretPosition.NextLine]);
        lines.Delete(CaretPosition.NextLine);
      }
    }

    public void DeleteBackwards()
    {
      if (CaretPosition != Position.BufferStart)
      {
        CharLeft();
        Delete();
        scrollView = scrollView.CheckLineUp(CaretPosition)
                               .CheckHorizontalScroll(CaretPosition);
      }
    }

    public void CharLeft()
    {
      desiredColumn = null;
      if (CaretPosition.Column > 0)
      {
        CaretPosition -= Position.OneColumn;
      }
      else if (CaretPosition.Line > 0)
      {
        CaretPosition = new Position(CaretPosition.PreviousLine, column: lines[CaretPosition.PreviousLine].Length);
      }
      scrollView = scrollView.CheckLineUp(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void WordLeft()
    {
      desiredColumn = null;
      if (CaretPosition.Column > 0)
      {
        int iStart = CaretBufferIndex;
        int iMin = lines[CaretPosition.Line].Start;
        int i = iStart;
        while (i > iMin && char.IsWhiteSpace(gapBuffer[i - 1]))
        {
          i--;
        }
        while (i > iMin && !char.IsWhiteSpace(gapBuffer[i - 1]))
        {
          i--;
        }
        CaretPosition -= new Position(0, iStart - i);
      }
      else if (CaretPosition.PreviousLine >= 0)
      {
        CaretPosition = new Position(CaretPosition.PreviousLine, column: lines[CaretPosition.PreviousLine].Length);
      }
      scrollView = scrollView.CheckLineUp(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    public void CharRight()
    {
      desiredColumn = null;
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        CaretPosition += Position.OneColumn;
      }
      else if (CaretPosition.NextLine < lines.Count)
      {
        CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      }
      scrollView = scrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void WordRight()
    {
      desiredColumn = null;
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        // Caret is not at the end of the line.
        int iStart = CaretBufferIndex;
        int iMax = lines[CaretPosition.Line].End;
        int i = iStart;
        while (i < iMax && !char.IsWhiteSpace(gapBuffer[i]))
        {
          i++;
        }
        while (i < iMax && char.IsWhiteSpace(gapBuffer[i]))
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
      scrollView = scrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public void MoveToLine(int line)
    {
      if (line.IsInRange(0, lines.Count))
      {
        desiredColumn = desiredColumn ?? CaretPosition.Column;
        int nextColumn = Math.Min(lines[line].Length, desiredColumn.Value);
        CaretPosition = new Position(line, nextColumn);
      }
    }

    public void LineUp()
    {
      MoveToLine(CaretPosition.PreviousLine);
      scrollView = scrollView.CheckLineUp(CaretPosition);
    }

    /// <summary>
    /// Go up no more than maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Number of lines to go up.</param>
    public void LineUp(int maxLines)
    {
      int deltaTextView = Min(CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line - deltaTextView);
      scrollView = scrollView.CheckPageUp(CaretPosition);
    }

    public void PageUp()
    {
      LineUp(ScrollView.Lines);
    }

    public void LineDown()
    {
      MoveToLine(CaretPosition.NextLine);
      scrollView = scrollView.CheckLineDown(CaretPosition);
    }

    /// <summary>
    /// Go down no more that maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Maximum number of lines to go down.</param>
    public void LineDown(int maxLines)
    {
      int deltaTextView = Min(LastLine - CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line + deltaTextView);
      scrollView = scrollView.CheckPageDown(CaretPosition);
    }

    public void PageDown()
    {
      LineDown(ScrollView.Lines);
    }

    public void LineStart()
    {
      desiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: 0);
      scrollView = scrollView.CheckHorizontalScroll(CaretPosition);
    }

    public void LineEnd()
    {
      desiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: lines[CaretPosition.Line].Length);
      scrollView = scrollView.CheckLineDown(CaretPosition)
                             .CheckHorizontalScroll(CaretPosition);
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      for (var line = 0; line < lines.Count; line++)
      {
        for (int i = lines[line].Start; i < lines[line].End; i++)
        {
          sb.Append(gapBuffer[i]);
        }
        if (line + 1 < lines.Count)
        {
          sb.Append(Environment.NewLine);
        }
      }
      return sb.ToString();
    }

    public IEnumerable<string> EnumerateLines()
    {
      var sb = new StringBuilder();
      int lineStart = scrollView.LineStart;
      int lineEnd = Min(lineStart + scrollView.Lines, lines.Count);
      for (var line = lineStart; line < lineEnd; line++)
      {
        sb.Clear();
        int iStart = lines[line].Start + scrollView.ColumnStart;
        int iEnd = Min(iStart + scrollView.Columns, lines[line].End);
        for (int i = iStart; i < iEnd; i++)
        {
          sb.Append(gapBuffer[i]);
        }
        yield return sb.ToString();
      }
    }
  }
}

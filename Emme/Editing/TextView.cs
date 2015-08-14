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
      private set { caretPosition = value; }
    }

    /// <summary>
    /// The number of lines this TextView contains.
    /// </summary>
    public int LastLine => lines.Count - 1;

    private void Shift(int delta)
    {
      lines[CaretPosition.Line] = lines[CaretPosition.Line].MoveEnd(delta);
      for (int i = CaretPosition.Line + 1; i < lines.Count; i++)
      {
        lines[i] = lines[i].Move(delta);
      }
    }

    static int CaretIndex(Span line, Position caretPosition)
    {
      return line.Start + caretPosition.Column;
    }

    public void Insert(char value)
    {
      desiredColumn = null;
      int insertIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
      gapBuffer.Insert(insertIndex, value);
      Shift(1);
      CaretPosition = new Position(CaretPosition.Line, CaretPosition.NextColumn);
    }

    public void InsertNewLine()
    {
      desiredColumn = null;
      int splitIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
      Tuple<Span, Span> splitSpans = lines[CaretPosition.Line].Split(splitIndex);
      lines[CaretPosition.Line] = splitSpans.Item1;
      CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      lines.Insert(CaretPosition.Line, splitSpans.Item2);
    }

    public void Delete()
    {
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        int deleteIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
        gapBuffer.Delete(deleteIndex);
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
      }
    }

    public void CharLeft()
    {
      desiredColumn = null;
      if (CaretPosition.Column > 0)
      {
        CaretPosition = new Position(CaretPosition.Line, CaretPosition.PreviousColumn);
      }
      else if (CaretPosition.Line > 0)
      {
        CaretPosition = new Position(CaretPosition.PreviousLine, column: lines[CaretPosition.PreviousLine].Length);
      }
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    public void CharRight()
    {
      desiredColumn = null;
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        CaretPosition = new Position(CaretPosition.Line, CaretPosition.NextColumn);
      }
      else if (CaretPosition.NextLine < lines.Count)
      {
        CaretPosition = new Position(CaretPosition.NextLine, column: 0);
      }
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
    }

    /// <summary>
    /// Go up no more than maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Number of lines to go up.</param>
    public void LineUp(int maxLines)
    {
      int deltaTextView = Min(CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line - deltaTextView);
    }

    public void LineDown()
    {
      MoveToLine(CaretPosition.NextLine);
    }

    /// <summary>
    /// Go down no more that maxLines number of lines.
    /// </summary>
    /// <param name="maxLines">Maximum number of lines to go down.</param>
    public void LineDown(int maxLines)
    {
      int deltaTextView = Min(LastLine - CaretPosition.Line, maxLines);
      MoveToLine(CaretPosition.Line + deltaTextView);
    }

    public void LineStart()
    {
      desiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: 0);
    }

    public void LineEnd()
    {
      desiredColumn = null;
      CaretPosition = new Position(CaretPosition.Line, column: lines[CaretPosition.Line].Length);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
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

    public IEnumerable<string> EnumerateLines(ScrollView scrollView)
    {
      var sb = new StringBuilder();
      for (var line = scrollView.LineStart; (line - scrollView.LineStart) < scrollView.Lines && line < lines.Count; line++)
      {
        sb.Clear();
        for (int i = lines[line].Start + scrollView.ColumnStart; i < lines[line].End; i++)
        {
          sb.Append(gapBuffer[i]);
        }
        yield return sb.ToString();
      }
    }
  }
}

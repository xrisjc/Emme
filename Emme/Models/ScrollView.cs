//
// File: GapBuffer.cs
//
// Copyright (C) 2010  Christopher Cowan
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

using static System.Math;

namespace Emme.Models
{
  /// <summary>
  /// Scruct that defines where the scrolling page is.
  /// </summary>
  struct ScrollView
  {
    public ScrollView(int lineStart, int leftColumn, int lines, int columns)
    {
      LineStart = lineStart;
      LeftColumn = leftColumn;
      Lines = lines;
      Columns = columns;
    }

    public int LineStart { get; }

    public int LeftColumn { get; }

    /// <summary>
    /// Total number of lines displayed in this ScrollView.
    /// </summary>
    public int Lines { get; }

    public int Columns { get; }

    /// <summary>
    /// Ending line of this ScrollView. This is non-inclusive. That is this is
    /// the first line below the ScrollView.
    /// </summary>
    public int LineEnd => LineStart + Lines;

    public ScrollView CheckLineDown(Position caretPosition)
    {
      if (caretPosition.Line >= LineEnd)
      {
        return HorizontalScroll(1);
      }
      else
      {
        return this;
      }
    }

    public ScrollView CheckLineUp(Position caretPosition)
    {
      if (caretPosition.Line < LineStart)
      {
        return HorizontalScroll(-1);
      }
      else
      {
        return this;
      }
    }

    public ScrollView CheckPageDown(Position caretPosition)
    {
      if (caretPosition.Line.IsInRange(LineStart, LineEnd))
      {
        return this;
      }
      else
      {
        return HorizontalScroll(Lines);
      }
    }

    public ScrollView CheckPageUp(Position caretPosition)
    {
      if (caretPosition.Line.IsInRange(LineStart, LineEnd))
      {
        return this;
      }
      else
      {
        int scrollLinesDelta = Min(LineStart, Lines);
        return HorizontalScroll(-scrollLinesDelta);
      }
    }

    public ScrollView HorizontalScroll(int delta) =>
      new ScrollView(LineStart + delta, LeftColumn, Lines, Columns);

    public Position PositionInView(Position positionInFile) =>
      new Position(positionInFile.Line - LineStart, positionInFile.Column - LeftColumn);
  }
}

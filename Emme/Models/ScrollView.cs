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
    public ScrollView(int lineStart, int columnStart, int lines, int columns)
    {
      LineStart = lineStart;
      ColumnStart = columnStart;
      Lines = lines;
      Columns = columns;
    }

    public int LineStart { get; }

    public int ColumnStart { get; }

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

    public int ColumnEnd => ColumnStart + Columns;

    /// <summary>
    /// Handles any scrolling needed after a line down command.
    /// </summary>
    /// <param name="caret">Caret position after a line down.</param>
    /// <returns>Updated ScrollView.</returns>
    public ScrollView CheckLineDown(Position caret)
    {
      if (caret.Line >= LineEnd)
      {
        return MoveWithLastLineAt(caret.Line);
      }
      else
      {
        return this;
      }
    }

    public ScrollView CheckHorizontalScroll(Position caret)
    {
      int horizontalScrollPad = Columns / 5;
      if (caret.Column < ColumnStart)
      {
        // Caret is to the left of the view.
        int newColumnStart = Max(caret.Column - horizontalScrollPad, 0);
        return new ScrollView(LineStart, newColumnStart, Lines, Columns);
      }
      else if (caret.Column >= ColumnEnd)
      {
        // Carte is to the right of the view.
        int newColumnStart = Max(caret.Column - Columns + horizontalScrollPad, 0);
        return new ScrollView(LineStart, newColumnStart, Lines, Columns);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Handles any scrolling needed after a line up command.
    /// </summary>
    /// <param name="caret">Caret position after a line up.</param>
    /// <returns>Update ScrollView.</returns>
    public ScrollView CheckLineUp(Position caret)
    {
      if (caret.Line >= LineEnd)
      {
        return MoveWithLastLineAt(caret.Line);
      }
      else if (caret.Line < LineStart)
      {
        return MoveToLine(caret.Line);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Handles any scrolling needed after a page down command.
    /// </summary>
    /// <param name="caret">Caret position after page down.</param>
    /// <returns>Updated ScrollView.</returns>
    public ScrollView CheckPageDown(Position caret)
    {
      if (caret.Line >= LineEnd + Lines)
      {
        return MoveWithLastLineAt(caret.Line);
      }
      else if (caret.Line >= LineEnd)
      {
        return MoveToLine(LineEnd);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Handles any scrolling needed after a page up command.
    /// </summary>
    /// <param name="caret">Caret position after page up.</param>
    /// <returns>Updated ScrollView.</returns>
    public ScrollView CheckPageUp(Position caret)
    {
      if (caret.Line >= LineEnd)
      {
        return MoveWithLastLineAt(caret.Line);
      }
      else if (caret.Line < LineStart)
      {
        // I don't expect caretPosition to be more than a page above
        // this PageView.
        return MoveToLine(Max(LineStart - Lines, 0));
      }
      else
      {
        return this;
      }
    }

    private ScrollView MoveToLine(int line) =>
      new ScrollView(line, ColumnStart, Lines, Columns);

    private ScrollView MoveWithLastLineAt(int line) =>
      MoveToLine(line - Lines + 1);

    public Position PositionInView(Position positionInFile) =>
      new Position(positionInFile.Line - LineStart, positionInFile.Column - ColumnStart);
  }
}

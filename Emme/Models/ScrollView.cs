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
  public class ScrollView
  {
    /// <summary>
    /// Line on which the ScrollView starts, inclusive.
    /// </summary>
    public int LineStart { get; private set; }

    /// <summary>
    /// Column on which the ScrollView starts, inclusive.
    /// </summary>
    public int ColumnStart { get; private set; }

    /// <summary>
    /// Total number of lines displayed in this ScrollView.
    /// </summary>
    public int Lines { get; private set; }

    /// <summary>
    /// Total number of columns displayed in this ScrollView.
    /// </summary>
    public int Columns { get; private set; }

    /// <summary>
    /// Primary constructor.
    /// </summary>
    /// <param name="lineStart">Line on which the ScrollView starts, inclusive.</param>
    /// <param name="columnStart">Column on which the ScrollView starts, inclusive.</param>
    /// <param name="lines">Total number of lines displayed in this ScrollView.</param>
    /// <param name="columns">Total number of columns displayed in this ScrollView.</param>
    public ScrollView(int lineStart, int columnStart, int lines, int columns)
    {
      LineStart = lineStart;
      ColumnStart = columnStart;
      Lines = lines;
      Columns = columns;
    }

    /// <summary>
    /// Line on which the scroll view ends, inclusive.
    /// </summary>
    private int LineEnd
    {
      get { return LineStart + Lines - 1; }
      set { LineStart = value - Lines + 1; }
    }

    /// <summary>
    /// Ending column of this ScrollView. This is non-inclusive. That is
    /// this is the first column right of the ScrollView.
    /// </summary>
    public int ColumnEnd => ColumnStart + Columns;

    /// <summary>
    /// Resizes the scroll view, while keeping the top left corner in place.
    /// </summary>
    /// <param name="lines">Number of lines in new view.</param>
    /// <param name="columns">Number of columns in new view.</param>
    /// <returns>Scroll view resized to given dimensions.</returns>
    public void ResizedTo(int lines, int columns)
    {
      Lines = lines;
      Columns = columns;
    }

    public ScrollView CheckHorizontalScroll(Position caret)
    {
      int horizontalScrollPad = Columns / 5;
      if (caret.Column < ColumnStart)
      {
        // Caret is to the left of the view.
        ColumnStart = Max(caret.Column - horizontalScrollPad, 0);
      }
      else if (caret.Column >= ColumnEnd)
      {
        // Carte is to the right of the view.
        ColumnStart = Max(caret.Column - Columns + horizontalScrollPad, 0);
      }
      return this;
    }

    public ScrollView CheckVerticalScroll(Position caret)
    {
      if (caret.Line < LineStart)
      {
        LineStart = caret.Line;
      }
      else if (caret.Line > LineEnd)
      {
        LineEnd = caret.Line;
      }
      return this;
    }

    /// <summary>
    /// Handles any scrolling needed after a line down command.
    /// </summary>
    /// <param name="caret">Caret position after a line down.</param>
    /// <returns>This object to allow method chaining.</returns>
    public ScrollView CheckLineDown(Position caret)
    {
      if (caret.Line > LineEnd)
      {
        LineEnd = caret.Line;
      }
      return this;
    }

    /// <summary>
    /// Handles any scrolling needed after a line up command.
    /// </summary>
    /// <param name="caret">Caret position after a line up.</param>
    /// <returns>This object to allow method chaining.</returns>
    public ScrollView CheckLineUp(Position caret)
    {
      if (caret.Line > LineEnd)
      {
        LineEnd = caret.Line;
      }
      else if (caret.Line < LineStart)
      {
        LineStart = caret.Line;
      }
      return this;
    }

    /// <summary>
    /// Handles any scrolling needed after a page down command.
    /// </summary>
    /// <param name="caret">Caret position after page down.</param>
    /// <returns>This object to allow method chaining.</returns>
    public ScrollView CheckPageDown(Position caret)
    {
      if (caret.Line > LineEnd + Lines)
      {
        // If the caret is below the scroll view, and would still be
        // below the scroll view after a page down. Could happen if
        // you resize the window to be small enough.
        LineEnd = caret.Line;
      }
      else if (caret.Line > LineEnd)
      {
        LineStart = LineEnd + 1;
      }
      return this;
    }

    /// <summary>
    /// Handles any scrolling needed after a page up command.
    /// </summary>
    /// <param name="caret">Caret position after page up.</param>
    /// <returns>This object to allow method chaining.</returns>
    public ScrollView CheckPageUp(Position caret)
    {
      if (caret.Line > LineEnd)
      {
        LineEnd = caret.Line;
      }
      else if (caret.Line < LineStart)
      {
        // I don't expect caretPosition to be more than a page above
        // this PageView.
        LineStart = Max(LineStart - Lines, 0);
      }
      return this;
    }

    public Position PositionInView(Position positionInFile)
      => new Position(positionInFile.Line - LineStart, positionInFile.Column - ColumnStart);
  }
}

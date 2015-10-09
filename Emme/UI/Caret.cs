//
// File: Caret.cs
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

using Emme.Models;

namespace Emme.UI
{
  /// <summary>
  /// Caret's size and position in pixels.
  /// </summary>
  struct Caret
  {
    /// <summary>
    /// Top-left x poision of the caret in pixels.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Caret's top-left y position in pixels.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Caret's width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Caret's height in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="height">Height of caret (font height).</param>
    public Caret(int x, int y, int width, int height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    /// <summary>
    /// Construct caret's position based on a logical caret's state.
    /// </summary>
    /// <param name="logicalCaretPosition">
    /// Logical caret position, i.e. in rows and columns of text.
    /// </param>
    /// <param name="fontMetrics">Current font's metrics.</param>
    /// <remarks>Assumes monospaced font is being used.</remarks>
    public Caret(Position logicalCaretPosition, FontMetrics fontMetrics)
      : this(
          x: logicalCaretPosition.Column * fontMetrics.Width + fontMetrics.Padding,
          y: logicalCaretPosition.Line * fontMetrics.Height,
          width: 1,
          height: fontMetrics.Height)
    {
    }
  }
}
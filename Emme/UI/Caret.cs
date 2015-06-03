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

using System.Drawing;
using Emme.Models;

namespace Emme.UI
{
  /// <summary>
  /// Caret's size and position.
  /// </summary>
  class Caret
  {
    /// <summary>
    /// Caret's width in pixels.
    /// </summary>
    const int WIDTH = 1;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="height">Height of caret (font height).</param>
    public Caret(int x, int y, int height)
    {
      Position = new Point(x, y);
      Size = new Size(WIDTH, height);
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
          height: fontMetrics.Height)
    {
    }

    /// <summary>
    /// Caret's position in pixels.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// Caret's size in pixels.
    /// </summary>
    public Size Size { get; }
  }
}
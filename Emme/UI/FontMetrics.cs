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

namespace Emme.UI
{
  /// <summary>
  /// Computes and stores font metrics for monospaced fonts.
  /// </summary>
  struct FontMetrics
  {
    /// <summary>
    /// Width in pixels of a character glyph.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Height of pixels of a character glyph.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Amount of padding, in pixels, put before or after text when drawn.
    /// </summary>
    public int Padding { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="width">Width in pixels of a character glyph.</param>
    /// <param name="height">Height of pixels of a character glyph.</param>
    /// <param name="padding">Amount of padding, in pixels, put before or after text when drawn.</param>
    public FontMetrics(int width, int height, int padding)
    {
      Width = width;
      Height = height;
      Padding = padding;
    }
  }
}
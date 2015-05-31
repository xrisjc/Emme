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
using System.Windows.Forms;

namespace Emme.UI
{
  /// <summary>
  /// Computes and stores font metrics for monospaced fonts.
  /// </summary>
  struct FontMetrics
  {
    readonly Size fontSize;
    readonly int padding;

    /// <summary>
    /// Width and height, in pixels, of a character glyph.
    /// </summary>
    public Size FontSize
    {
      get { return fontSize; }
    }

    /// <summary>
    /// Amount of padding, in pixels, put before or after text when drawn.
    /// </summary>
    public int Padding
    {
      get { return padding; }
    }

    /// <summary>Constructor</summary>
    /// <param name="deviceContext">A device context</param>
    /// <param name="font">Font to measure. Assumed to be monospaced.</param>
    public FontMetrics(IDeviceContext deviceContext, Font font)
    {
      var purposedSize = new Size(short.MaxValue, short.MaxValue); // Bounding values

      // get a character's width without padding.
      fontSize = TextRenderer.MeasureText(deviceContext, "a", font, purposedSize, TextFormatFlags.NoPadding);

      // going to use font.Height instead b/c Petzold says that better for
      // formatting lines of text.
      // TODO: I should use font.GetHeight(grfx)
      fontSize.Height = font.Height;

      // Get how much we're padding the text.
      Size fontSizeWithPadding = TextRenderer.MeasureText(deviceContext, "a", font);
      padding = (fontSizeWithPadding.Width - fontSize.Width) / 2;
    }
  }
}
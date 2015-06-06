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

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emme.Editing;
using static Emme.UI.NativeMethods;
using Emme.Models;

namespace Emme.UI
{
  class EditorForm : Form
  {

    TextView textView;

    readonly FontMetrics fontMetrics;
    Caret caret;


    public EditorForm(TextView textView)
    {
      this.textView = textView;
      textView.CaretPositionChanged += TextView_CaretPositionChanged;

      Text = "Emme";

      ForeColor = SystemColors.WindowText;
      BackColor = SystemColors.Window;

      Font = new Font("Consolas", 10f);
      fontMetrics = CreateFontMetrics();
      caret = new Caret(Position.BufferStart, fontMetrics);
      Width = 80 * fontMetrics.Width + 2 * fontMetrics.Padding; // display as 80 x 40 grid.
      Height = 40 * fontMetrics.Height;


      // Flickering be gone. Got the method from here:
      // http://stackoverflow.com/questions/8046560/how-to-stop-flickering-c-sharp-winforms
      SetStyle(
        flag: ControlStyles.UserPaint | 
          ControlStyles.OptimizedDoubleBuffer |
          ControlStyles.AllPaintingInWmPaint,
        value: true);
    }

    /// <summary>
    /// Creates an appropriate FontMetrics value for the currently set Font.
    /// </summary>
    private FontMetrics CreateFontMetrics()
    {
      using (Graphics graphics = CreateGraphics())
      {
        var purposedSize = new Size(short.MaxValue, short.MaxValue); // Bounding values

        // get a character's width without padding.
        Size fontSize = TextRenderer.MeasureText(graphics, "a", Font, purposedSize, TextFormatFlags.NoPadding);

        // going to use font.Height instead b/c Petzold says that better for
        // formatting lines of text.
        // TODO: I should use font.GetHeight(grfx)
        fontSize = new Size(fontSize.Width, Font.Height);


        // Get how much we're padding the text.
        Size fontSizeWithPadding = TextRenderer.MeasureText(graphics, "a", Font);
        int padding = (fontSizeWithPadding.Width - fontSize.Width) / 2;

        return new FontMetrics(fontSize.Width, fontSize.Height, padding);
      }
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      CreateCaret(this.Handle, SOLID_BITMAP_HANDLE, caret.Width, caret.Height);
      SetCaretPos(caret.X, caret.Y);
      ShowCaret(this.Handle);
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      DestroyCaret();
    }

    private void TextView_CaretPositionChanged(object sender, PositionEventArgs e)
    {
      caret = new Caret(e.Position, fontMetrics);
      SetCaretPos(caret.X, caret.Y);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Control && (e.KeyCode == Keys.S))
      {
        using (var dialog = new SaveFileDialog())
        {
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            using (var stream = dialog.OpenFile())
            {
              using (var writer = new StreamWriter(stream))
              {
                writer.Write(textView.ToString());
              }
            }
          }
        }
      }
      else switch (e.KeyCode)
      {
        case Keys.Enter:
          textView.InsertNewLine();
          break;

        case Keys.Back:
          textView.DeleteBackwards();
          break;

        case Keys.Delete:
          textView.Delete();
          break;

        case Keys.Left:
          textView.CharLeft();
          break;

        case Keys.Right:
          textView.CharRight();
          break;

        case Keys.Up:
          textView.LineUp();
          break;

        case Keys.Down:
          textView.LineDown();
          break;

        case Keys.Home:
          textView.LineStart();
          break;

        case Keys.End:
          textView.LineEnd();
          break;
      }

      Invalidate();
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      base.OnKeyPress(e);

      if (char.IsControl(e.KeyChar))
      {
        return;
      }

      textView.Insert(e.KeyChar);
      
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var point = new Point(0, 0);

      foreach (string line in textView)
      {
        TextRenderer.DrawText(
            e.Graphics,
            line,
            Font,
            point,
            ForeColor,
            TextFormatFlags.NoPrefix); // No prefix, or ampersands won't show up.

        point.Y += fontMetrics.Height;
      }
    }
  }
}
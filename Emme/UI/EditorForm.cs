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
using System.Windows.Forms;
using Emme.Editing;
using static Emme.UI.Win32;

namespace Emme.UI
{
  class EditorForm : Form
  {
    /// <summary>
    /// Bitmap handle to create a solid color caret.
    /// </summary>
    const int SOLID_BITMAP_HANDLE = 0;

    TextView textView;

    readonly FontMetrics fontMetrics;
    Caret caret;


    public EditorForm(TextView textView)
    {
      this.textView = textView;
      textView.CaretPositionChanged += TextView_CaretPositionChanged;

      Text = "Emme";

      Font = new Font("Consolas", 10f);

      ForeColor = SystemColors.WindowText;
      BackColor = SystemColors.Window;


      using (Graphics graphics = CreateGraphics())
      {
        fontMetrics = new FontMetrics(graphics, Font);
      }

      caret = new Caret(textView.CaretPosition, fontMetrics);



      Width = 80 * fontMetrics.FontSize.Width + 2 * fontMetrics.Padding; // display as 80 x 40 grid.
      Height = 40 * fontMetrics.FontSize.Height;


      // Flickering be gone. Got the method from here:
      // http://stackoverflow.com/questions/8046560/how-to-stop-flickering-c-sharp-winforms
      SetStyle(
        flag: ControlStyles.UserPaint | 
          ControlStyles.OptimizedDoubleBuffer |
          ControlStyles.AllPaintingInWmPaint,
        value: true);
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      CreateCaret(this.Handle, SOLID_BITMAP_HANDLE, caret.Size.Width, caret.Size.Height);
      SetCaretPos(caret.Position.X, caret.Position.Y);
      ShowCaret(this.Handle);
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      DestroyCaret();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      switch (e.KeyCode)
      {
        case Keys.Enter:
          textView.InsertNewLine();
          break;

        case Keys.Back:
          textView.DeletePrevious();
          break;

        case Keys.Delete:
          textView.Delete();
          break;

        case Keys.Left:
          textView.MoveToPrevious();
          break;

        case Keys.Right:
          textView.MoveToNext();
          break;

        case Keys.Up:
          textView.MoveToPreviousLine();
          break;

        case Keys.Down:
          textView.MoveToNextLine();
          break;

        case Keys.Home:
          textView.MoveToLineStart();
          break;

        case Keys.End:
          textView.MoveToLineEnd();
          break;
      }

      Invalidate();
    }

    private void TextView_CaretPositionChanged(object sender, PositionEventArgs e)
    {
      caret = new Caret(e.Position, fontMetrics);
      SetCaretPos(caret.Position.X, caret.Position.Y);
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

        point.Y += fontMetrics.FontSize.Height;
      }
    }
  }
}
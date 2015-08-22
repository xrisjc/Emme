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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emme.Editing;
using Emme.Models;
using static System.Math;
using static Emme.UI.NativeMethods;

namespace Emme.UI
{
  class EditorForm : Form
  {

    TextView textView;
    ScrollView scrollView = new ScrollView(lineStart: 0, columnStart: 0, lines: 24, columns: 80);

    readonly FontMetrics fontMetrics;
    Caret caret;


    public EditorForm()
    {
      this.textView = new TextView();

      Text = "Emme";

      ForeColor = SystemColors.WindowText;
      BackColor = SystemColors.Window;

      Font = new Font("Consolas", 10f);
      fontMetrics = CreateFontMetrics();
      caret = new Caret(Position.BufferStart, fontMetrics);

      ClientSize =
        new Size(
          scrollView.Columns * fontMetrics.Width + 2 * fontMetrics.Padding,
          scrollView.Lines * fontMetrics.Height);


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

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);

      int lines = ClientSize.Height / fontMetrics.Height;
      int columns = (ClientSize.Width - 2 * fontMetrics.Padding) / fontMetrics.Width;

      scrollView = new ScrollView(scrollView.LineStart, scrollView.ColumnStart, lines, columns);

      Invalidate();
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      CreateCaret(Handle, SOLID_BITMAP_HANDLE, caret.Width, caret.Height);
      SetCaretPos(caret.X, caret.Y);
      ShowCaret(Handle);
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      DestroyCaret();
    }

    private void UpdateCaretPosition(Position position)
    {
      caret = new Caret(scrollView.PositionInView(position), fontMetrics);
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
      else if (e.Control && (e.KeyCode == Keys.O))
      {
        using (var dialog = new OpenFileDialog())
        {
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            using (var stream = dialog.OpenFile())
            {
              using (var reader = new StreamReader(stream))
              {
                string fileContent = reader.ReadToEnd();
                textView = new TextView(fileContent);
                UpdateCaretPosition(textView.CaretPosition);
              }
            }
          }
        }
      }
      else
      {
        switch (e.KeyCode)
        {
          case Keys.Enter:
            textView.InsertNewLine();
            scrollView = scrollView.CheckLineDown(textView.CaretPosition);
            break;

          case Keys.Back:
            textView.DeleteBackwards();
            scrollView = scrollView.CheckLineUp(textView.CaretPosition)
                                   .CheckHorizontalScroll(textView.CaretPosition);
            break;

          case Keys.Delete:
            textView.Delete();
            break;

          case Keys.Left:
            if (e.Control)
            {
              textView.WordLeft();
            }
            else
            {
              textView.CharLeft();
            }
            scrollView = scrollView.CheckLineUp(textView.CaretPosition)
                                   .CheckHorizontalScroll(textView.CaretPosition);
            break;

          case Keys.Right:
            if (e.Control)
            {
              textView.WordRight();
            }
            else
            {
              textView.CharRight();
            }
            scrollView = scrollView.CheckLineDown(textView.CaretPosition)
                                   .CheckHorizontalScroll(textView.CaretPosition);
            break;

          case Keys.Up:
            textView.LineUp();
            scrollView = scrollView.CheckLineUp(textView.CaretPosition);
            break;

          case Keys.Down:
            textView.LineDown();
            scrollView = scrollView.CheckLineDown(textView.CaretPosition);
            break;

          case Keys.Home:
            textView.LineStart();
            scrollView = scrollView.CheckHorizontalScroll(textView.CaretPosition);
            break;

          case Keys.End:
            textView.LineEnd();
            scrollView = scrollView.CheckLineDown(textView.CaretPosition)
                                   .CheckHorizontalScroll(textView.CaretPosition);
            break;

          case Keys.PageDown:
            textView.LineDown(scrollView.Lines);
            scrollView = scrollView.CheckPageDown(textView.CaretPosition);
            break;

          case Keys.PageUp:
            textView.LineUp(scrollView.Lines);
            scrollView = scrollView.CheckPageUp(textView.CaretPosition);
            break;
        }
        UpdateCaretPosition(textView.CaretPosition);
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
      scrollView = scrollView.CheckHorizontalScroll(textView.CaretPosition);
      UpdateCaretPosition(textView.CaretPosition);

      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var point = new Point(0, 0);

      foreach (string line in textView.EnumerateLines(scrollView))
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
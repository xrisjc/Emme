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

using Emme.Editing;
using Emme.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Emme.Editing.EditCommand;
using static Emme.UI.NativeMethods;
using static System.Math;

namespace Emme.UI
{
    class EditorForm : Form
    {

        TextView textView;
        Stack<IEditCommand> undos = new Stack<IEditCommand>();
        Stack<IEditCommand> redos = new Stack<IEditCommand>();

        readonly FontMetrics fontMetrics;
        Caret caret;


        public EditorForm()
        {
            textView = new TextViewFactoryNew().Create();

            Text = "Emme";

            ForeColor = SystemColors.WindowText;
            BackColor = SystemColors.Window;

            Font = new Font("Consolas", 10f);
            fontMetrics = CreateFontMetrics();
            caret = new Caret(textView.Caret, fontMetrics);

            ClientSize =
              new Size(
                textView.ScrollView.Columns * fontMetrics.Width + 2 * fontMetrics.Padding,
                (textView.ScrollView.Lines + 1) * fontMetrics.Height);


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

            int lines = ClientSize.Height / fontMetrics.Height - 1;
            int columns = (ClientSize.Width - 2 * fontMetrics.Padding) / fontMetrics.Width;

            textView.ResizeScrollView(lines, columns);

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
            caret = new Caret(textView.ScrollView.PositionInView(position), fontMetrics);
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
                        undos.Clear();
                        redos.Clear();
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
                                textView = new TextViewFactoryFile(fileContent).Create();
                                UpdateCaretPosition(textView.Caret);
                            }
                        }
                        undos.Clear();
                        redos.Clear();
                    }
                }
            }
            else if (e.Control && (e.KeyCode == Keys.Z))
            {
                undos.Undo(textView, redos);
                UpdateCaretPosition(textView.Caret);
            }
            else if (e.Control && (e.KeyCode == Keys.Y))
            {
                redos.Undo(textView, undos);
                UpdateCaretPosition(textView.Caret);
            }
            else
            {
                IEditCommand editCommand = null;
                switch (e.KeyCode)
                {
                    case Keys.J:
                        if (e.Control)
                        {
                            editCommand = new EditCommandJoinLines();
                        }
                        break;
                    case Keys.Enter:
                        editCommand = new EditCommandInsertNewLine();
                        break;

                    case Keys.Back:
                        editCommand = new EditCommandDeleteBackwards();
                        break;

                    case Keys.Delete:
                        editCommand = new EditCommandDelete();
                        break;

                    case Keys.Left:
                        if (e.Control)
                        {
                            editCommand = new EditCommandWordLeft();
                        }
                        else
                        {
                            editCommand = new EditCommandCharLeft();
                        }
                        break;

                    case Keys.Right:
                        if (e.Control)
                        {
                            editCommand = new EditCommandWordRight();
                        }
                        else
                        {
                            editCommand = new EditCommandCharRight();
                        }
                        break;

                    case Keys.Up:
                        editCommand = new EditCommandLineUp();
                        break;

                    case Keys.Down:
                        editCommand = new EditCommandDown();
                        break;

                    case Keys.Home:
                        editCommand = new EditCommandLineStart();
                        break;

                    case Keys.End:
                        editCommand = new EditCommandLineEnd();
                        break;

                    case Keys.PageDown:
                        editCommand = new EditCommandPageDown();
                        break;

                    case Keys.PageUp:
                        editCommand = new EditCommandPageUp();
                        break;
                }
                editCommand?.Execute(textView, undos, redos);
                UpdateCaretPosition(textView.Caret);
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

            Insert(e.KeyChar).Execute(textView, undos, redos);

            UpdateCaretPosition(textView.Caret);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw Content
            int i = 0;
            int j = 0;
            foreach (Text text in textView.EnumerateText())
            {
                int x = i * fontMetrics.Width;
                int y = j++ * fontMetrics.Height;
                TextRenderer.DrawText(
                    e.Graphics,
                    text.Content,
                    Font,
                    new Point(x, y),
                    ForeColor,
                    TextFormatFlags.NoPrefix); // No prefix, or ampersands won't show up.
            }

            // Draw Command Line
            var start = new Point(0, ClientSize.Height - fontMetrics.Height);
            var end = new Point(ClientSize.Width, start.Y);
            using (Pen p = new Pen(ForeColor))
            {
                e.Graphics.DrawLine(p, start, end);
            }
            string commandLine = $"View {textView.ScrollView} Ln {textView.Caret.Line} Col {textView.Caret.Column}";
            int padding = Max(textView.ScrollView.Columns - commandLine.Length, 0);
            commandLine = new string(' ', padding) + commandLine;
            TextRenderer.DrawText(
                e.Graphics,
                commandLine,
                Font,
                start,
                ForeColor,
                TextFormatFlags.NoPrefix);
        }
    }
}
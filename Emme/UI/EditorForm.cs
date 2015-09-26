using Emme.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Emme.UI.NativeMethods;

namespace Emme.UI
{
    public class EditorForm : Form
    {
        readonly FontMetrics fontMetrics;
        Caret caret;


        public EditorForm()
        {
            Text = "Emme";

            ForeColor = SystemColors.WindowText;
            BackColor = SystemColors.Window;

            Font = new Font("Consolas", 10f);
            fontMetrics = CreateFontMetrics();
            caret = new Caret(new Position(0, 0), fontMetrics);

            ClientSize =
              new Size(
                80 * fontMetrics.Width + 2 * fontMetrics.Padding,
                24 * fontMetrics.Height);


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
                    }
                }
            }
            else if (e.Control && (e.KeyCode == Keys.O))
            {
                using (var dialog = new OpenFileDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                    }
                }
            }
            else if (e.Control && (e.KeyCode == Keys.Z))
            {
            }
            else if (e.Control && (e.KeyCode == Keys.Y))
            {
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        break;

                    case Keys.Back:
                        break;

                    case Keys.Delete:
                        break;

                    case Keys.Left:
                        if (e.Control)
                        {
                        }
                        else
                        {
                        }
                        break;

                    case Keys.Right:
                        if (e.Control)
                        {
                        }
                        else
                        {
                        }
                        break;

                    case Keys.Up:
                        break;

                    case Keys.Down:
                        break;

                    case Keys.Home:
                        break;

                    case Keys.End:
                        break;

                    case Keys.PageDown:
                        break;

                    case Keys.PageUp:
                        break;
                }
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

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var point = new Point(0, 0);

            foreach (string line in new string[0])
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
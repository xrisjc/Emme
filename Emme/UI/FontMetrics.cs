using System.Drawing;
using System.Windows.Forms;

namespace Emme.UI
{
    /// <summary>
    /// Computes and stores font metrics for monospaced fonts.
    /// </summary>
    public class FontMetrics
    {
        /// <summary>
        /// Creates an appropriate FontMetrics value for control.
        /// </summary>
        /// <param name="control">A WinForms control.</param>
        public FontMetrics(Control control)
        {
            using (Graphics graphics = control.CreateGraphics())
            {
                // Bounding values
                var purposedSize = new Size(short.MaxValue, short.MaxValue);

                // Get a character's width without padding.
                Size fontSize = TextRenderer.MeasureText(graphics, "a",
                    control.Font, purposedSize, TextFormatFlags.NoPadding);

                // Using font.Height instead b/c Petzold says that better for
                // formatting lines of text.
                fontSize = new Size(fontSize.Width, control.Font.Height);


                // Get how much we're padding the text.
                Size fontSizeWithPadding = TextRenderer.MeasureText(graphics,
                    "a", control.Font);
                int padding = (fontSizeWithPadding.Width - fontSize.Width) / 2;

                Width = fontSize.Width;
                Height = fontSize.Height;
                Padding = padding;
            }
        }

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
        /// Calculates a ClientSize that will be large enough for the given
        /// number of lines and columns.
        /// </summary>
        /// <param name="lines">Number of visible lines.</param>
        /// <param name="columns">Number of visible columns.</param>
        /// <returns>A ClientSize value.</returns>
        public Size ClientSize(int lines, int columns) =>
            new Size(columns * Width + 2 * Padding, lines * Height);
    }
}
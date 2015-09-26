using Emme.Models;

namespace Emme.UI
{
    /// <summary>
    /// Caret's size and position in pixels.
    /// </summary>
    public struct Caret
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
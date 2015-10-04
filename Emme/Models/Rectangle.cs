namespace Emme.Models
{
    /// <summary>
    /// Represents a rectangular area of a grid of lines and columns.
    /// </summary>
    public struct Rectangle
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="line">
        /// Starting line, inclusive, of this Rectangle.
        /// </param>
        /// <param name="column">
        /// Starting column, inclusive, of this Rectangle.
        /// </param>
        /// <param name="height">
        /// Height in number of lines of this Rectangle.
        /// </param>
        /// <param name="width">
        /// Width in number of columns of this Rectangle.
        /// </param>
        public Rectangle(int line, int column, int height, int width)
        {
            Line = line;
            Column = column;
            Height = height;
            Width = width;
        }

        /// <summary>
        /// Starting line, inclusive, of this Rectangle.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Starting column, inclusive, of this Rectangle.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Height in number of lines of this Rectangle.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Width in number of columns of this Rectangle.
        /// </summary>
        public int Width { get; }
    }
}

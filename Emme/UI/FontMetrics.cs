namespace Emme.UI
{
    /// <summary>
    /// Computes and stores font metrics for monospaced fonts.
    /// </summary>
    public struct FontMetrics
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

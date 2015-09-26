namespace Emme.Models
{
    /// <summary>
    /// Holds line, column content position.
    /// </summary>
    /// <remarks>
    /// Both Line and Column will start from zero to avoid subtracting one
    /// everywhere. But in the UI they may be shown to start at 1.
    /// </remarks>
    public struct Position
    {
        /// <summary>
        /// Primary Constructor.
        /// </summary>
        /// <param name="line">Line number, starting from zero.</param>
        /// <param name="column">Column number, starting from zero.</param>
        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Line number, starting from zero.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Column number, starting from zero.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Override ToString for debugging purposes.
        /// </summary>
        /// <returns>A string with this Position's values.</returns>
        public override string ToString() => $"{{{Line}, {Column}}}";
    }
}
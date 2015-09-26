namespace Emme.Models
{
    /// <summary>
    /// Represents an interval of discreet items. The start is inclusive, and
    /// the end is exclusive. 
    /// </summary>
    public struct Span
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="start">Inclusive start of the span.</param>
        /// <param name="end">Exclusive end of the span.</param>
        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Inclusive start of the span.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Exclusive end of the span.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// The nmber of items in this Span.
        /// </summary>
        public int Length => End - Start;

        /// <summary>
        /// Override ToString for debugging purposes.
        /// </summary>
        /// <returns>String with this Span's values.</returns>
        public override string ToString() => $"{Start}, {End}";
    }
}
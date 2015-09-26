namespace Emme.Models
{
    /// <summary>
    /// Extension methods to the Gap struct.
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// Creates a new Gap by moving Start a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>New Gap object with new Start.</returns>
        public static Span MoveStart(this Span gap, int delta)
          => new Span(gap.Start + delta, gap.End);

        /// <summary>
        /// Creates a new Gap by moving End a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>New Gap object with new End.</returns>
        public static Span MoveEnd(this Span gap, int delta)
          => new Span(gap.Start, gap.End + delta);

        /// <summary>
        /// Creates a new gap by moving Start and End by a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>New Gap object with new Start and End.</returns>
        public static Span Move(this Span gap, int delta)
          => new Span(gap.Start + delta, gap.End + delta);
    }
}
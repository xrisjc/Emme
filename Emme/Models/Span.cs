namespace Emme.Models
{
    public struct Span
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Index in buffer where the gap starts and content ends.</param>
        /// <param name="end">Index in buffer where the gap ends and the rest of the content begins.</param>
        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Index in buffer where the gap starts and content ends.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Index in buffer where the gap ends and the rest of the content
        /// begins.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Length of the gap; the total available space left in buffer.
        /// </summary>
        public int Length => End - Start;

        /// <summary>
        /// Converts a index into a buffer's content -- which ignores the
        /// gap -- to index into the buffer.
        /// </summary>
        public int ToBufferIndex(int contentIndex)
        {
            return (contentIndex >= Start) ? contentIndex + Length : contentIndex;
        }
    }
}
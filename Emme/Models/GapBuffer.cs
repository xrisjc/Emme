using static System.Array;
using static System.Math;

namespace Emme.Models
{
    /// <summary>
    /// Buffer to allow efficient inserting and deleting of content by keeping
    /// a gap of available space where insertions and deletions happen.
    /// </summary>
    public class GapBuffer<T>
    {
        private T[] buffer;
        private Marker gap;

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="initialCapacity">Initial buffer capacity.</param>
        public GapBuffer(int initialCapacity = 256)
        {
            buffer = new T[initialCapacity];
            gap = new Marker(0, initialCapacity);
        }

        /// <summary>
        /// Count of values this GapBuffer contains.
        /// </summary>
        public int Count => buffer.Length - gap.Length;

        /// <summary>
        /// Indexer into this GapBuffer's content.
        /// </summary>
        /// <param name="index">Index in the interval [0, Count).</param>
        /// <returns>Value at index.</returns>
        public T this[int index] => buffer[ToBufferIndex(index)];

        /// <summary>
        /// Inserts a value into this GapBuffer at an index.
        /// </summary>
        /// <param name="index">Index in the interval [0, Count].</param>
        /// <param name="value">Value to insert.</param>
        public void InsertAt(int index, T value)
        {
            GrowBuffer(index, 1);
            MoveContent(index);
            buffer[index] = value;
            gap = new Marker(index + 1, index + gap.Length);
        }

        /// <summary>
        /// Deletes character at index.
        /// </summary>
        /// <param name="index">Index in interval [0, Count)</param>
        public void DeleteAt(int index)
        {
            MoveContent(index);
            gap = new Marker(index, index + gap.Length + 1);
        }

        /// <summary>
        /// Converts a index into this GapBuffers content -- which ignores the
        /// gap -- to index into buffer.
        /// </summary>
        /// <param name="index">Index in interval [0, Count)</param>
        private int ToBufferIndex(int index) =>
            (index >= gap.Start) ? index + gap.Length : index;

        /// <summary>
        /// Move content, if needed, such that the gap can start at index.
        /// </summary>
        /// <param name="index">Index in range [0, Count].</param>
        private void MoveContent(int index)
        {
            if (index < gap.Start)
            {
                Copy(buffer, index, buffer, index + gap.Length,
                    gap.Start - index);
            }
            else if (index > gap.Start)
            {
                Copy(buffer, gap.End, buffer, gap.Start, index - gap.Start);
            }
        }

        /// <summary>
        /// Grows buffer size, if needed. Increases size enough to fit
        /// minGapSize new values or grows buffer by 50%, which ever is the
        /// largest. If resize is done, then the new buffer will have the
        /// gap starting at index.
        /// </summary>
        /// <param name="index">Index in the interval [0, Count].</param>
        /// <param name="minGapSize">Minimum amout of space required.</param>
        private void GrowBuffer(int index, int minGapSize)
        {
            if (gap.Length == 0)
            {
                int newGapSize = Max(buffer.Length / 2, minGapSize);
                var newBuffer = new T[buffer.Length + newGapSize];
                var newGap = new Marker(index, index + newGapSize);
                Copy(buffer, newBuffer, index);
                Copy(buffer, index, newBuffer, newGap.End,
                    buffer.Length - index);
                buffer = newBuffer;
                gap = newGap;
            }
        }
    }
}
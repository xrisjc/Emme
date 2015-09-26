using System;

namespace Emme.Models
{
    /// <summary>
    /// Buffer to allow efficient inserting and deleting of content by keeping
    /// a gap of available space where insertions and deletions happen.
    /// </summary>
    public class GapBuffer<T> where T : struct
    {
        /// <summary>
        /// Actual value buffer.
        /// </summary>
        private T[] buffer;

        /// <summary>
        /// Gap indices.
        /// </summary>
        private Span gap;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialCapacity">Initial capacity of buffer.</param>
        public GapBuffer(int initialCapacity = 256)
        {
            buffer = new T[initialCapacity];

            // Gap is initially fills the buffer.
            gap = new Span(0, buffer.Length);
        }

        /// <summary>
        /// Length of content before the gap.
        /// </summary>
        private int LeftContentLength => gap.Start;

        /// <summary>
        /// Length of content after the gap.
        /// </summary>
        private int RightContentLength => buffer.Length - gap.End;

        /// <summary>
        /// Total number of values the internal buffer can hold before resizing.
        /// </summary>
        private int Capacity => buffer.Length;

        /// <summary>
        /// Total number of values actually contained in the gap buffer.
        /// </summary>
        public int Count => Capacity - gap.Length;

        /// <summary>
        /// Returns the value at the given content index. When 
        /// index == Count, then null is returned.
        /// </summary>
        public T this[int index]
        {
            get { return buffer[gap.ToBufferIndex(index)]; }
            set { buffer[gap.ToBufferIndex(index)] = value; }
        }

        /// <summary>
        /// Insert a value at the given index.
        /// </summary>
        /// <param name="index">Index in buffer where the value will be inserted.</param>
        /// <param name="value">Value to insert.</param>
        /// <returns>This object to allow method chaining.</returns>
        public GapBuffer<T> Insert(int index, T value)
        {
            MoveGapTo(index);

            if (gap.Length == 0)
                GrowBuffer(1);

            buffer[gap.Start] = value;
            gap = gap.MoveStart(1);

            return this;
        }

        /// <summary>
        /// Appends the value at the end of the buffer.
        /// </summary>
        /// <param name="value">Value to append.</param>
        /// <returns>This buffer to enable method chaining.</returns>
        public GapBuffer<T> Append(T value)
        {
            return Insert(Count, value);
        }

        /// <summary>
        /// Deletes content from the buffer at the current index. CurrentIndex
        /// remains the same.
        /// </summary>
        /// <param name="length">How many characters to delete.</param>
        public void Delete(int index, int length = 1)
        {
            MoveGapTo(index);
            gap = gap.MoveEnd(length);
        }

        /// <summary>
        /// Grow buffer size. Increases size enough to fit minGapSize new
        /// elements or grows buffer by 50%, which ever is largest.
        /// </summary>
        /// <param name="minGapSize">Minimum amout of room required.</param>
        private void GrowBuffer(int minGapSize)
        {
            // Take max of 50% larger buffer or content size + minGapSize
            int newCapacity = Math.Max(
                buffer.Length + buffer.Length / 2,
                Count + minGapSize);


            var newBuffer = new T[newCapacity];
            var newGap = new Span(gap.Start, gap.End + newBuffer.Length - buffer.Length);

            // Copy items before gap.
            Array.Copy(buffer, newBuffer, LeftContentLength);
            // Copy items after gap.
            Array.Copy(buffer, gap.End, newBuffer, newGap.End, RightContentLength);

            gap = newGap;
            buffer = newBuffer;
        }

        /// <summary>
        /// Move gap so that content could be added or deleted at given content
        /// index.
        /// </summary>
        /// <param name="index">Index in content.</param>
        private void MoveGapTo(int index)
        {
            // Index must be in [0, Count].
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");

            int delta = index - gap.Start;
            if (delta == 0) return;

            Span newGap = gap.Move(delta);
            int length = Math.Abs(delta);

            if (delta < 0)
            {
                // newGap.Start < gap.Start < newGap.End < gap.End
                // Move delta chars before old gap to after old gap
                Copy(newGap.Start, newGap.End, length);
            }
            else if (delta > 0)
            {
                // gap.Start < newGap.Start < gap.End < newGap.End
                // Move delta chars from end of old gap to before old gap
                Copy(gap.End, gap.Start, length);
            }

            gap = newGap;
        }

        /// <summary>
        /// Copies a range of elements in the buffer.
        /// </summary>
        /// <param name="sourceIndex">Buffer index to copy from.</param>
        /// <param name="destinationIndex">Buffer index to copy to.</param>
        /// <param name="length">Number of items to copy.</param>
        private void Copy(int sourceIndex, int destinationIndex, int length)
        {
            Array.Copy(buffer, sourceIndex, buffer, destinationIndex, length);
        }
    }
}
//
// File: GapBuffer.cs
//
// Copyright (C) 2010  Christopher Cowan
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emme.Models
{
  /// <summary>
  /// Buffer to allow efficient inserting and deleting of content by keeping
  /// a gap of available space where insertions and deletions happen.
  /// </summary>
  public class GapBuffer
  {
    /// <summary>
    /// Actual character buffer.
    /// </summary>
    char[] buffer;

    /// <summary>
    /// Gap indices.
    /// </summary>
    Span gap;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity of buffer.</param>
    public GapBuffer(int initialCapacity = 256)
    {
      buffer = new char[initialCapacity];

      // Gap is initially fills the buffer.
      gap = new Span(0, buffer.Length);
    }

    /// <summary>
    /// Constructor. Creates a buffer just large enough to hold the given
    /// initial content. That is, the gap will be of length zero.
    /// </summary>
    /// <param name="initialContent">Initial buffer content.</param>
    public GapBuffer(string initialContent)
    {
      buffer = initialContent.ToCharArray();
      // Gap start and end default to zero.
    }

    /// <summary>
    /// Length of content before the gap.
    /// </summary>
    int LeftContentLength
    {
      get { return gap.Start; }
    }

    /// <summary>
    /// Length of content after the gap.
    /// </summary>
    int RightContentLength
    {
      get { return buffer.Length - gap.End; }
    }

    /// <summary>
    /// Total number of characters the internal buffer can hold before
    /// resizing.
    /// </summary>
    public int Capacity
    {
      get { return buffer.Length; }
    }

    /// <summary>
    /// Total number of characters actually contained in the gap buffer.
    /// </summary>
    public int Count
    {
      get { return Capacity - gap.Length; }
    }

    /// <summary>
    /// Returns the character at the given content index. When 
    /// index == Count, then '\0' is returned.
    /// </summary>
    public char this[int index]
    {
      get
      {
        int bufferIndex = gap.ToBufferIndex(index);
        return bufferIndex == Count ? '\0' : buffer[bufferIndex];
      }
    }

    public void Insert(int index, char value)
    {
      MoveGapTo(index);

      if (gap.Length == 0)
        GrowBuffer(1);

      buffer[gap.Start] = value;
      gap = gap.MoveStart(1);
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
    void GrowBuffer(int minGapSize)
    {
      // Take max of 50% larger buffer or content size + minGapSize
      int newCapacity = Math.Max(
          buffer.Length + buffer.Length / 2,
          Count + minGapSize);


      var newBuffer = new char[newCapacity];
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
    void MoveGapTo(int index)
    {
      // Index must be in [0, Count].
      if (index < 0 || index > Count)
        throw new ArgumentOutOfRangeException("index");

      int delta = index - gap.Start;
      if (delta == 0) return;

      Span newGap = gap.Move(delta);
      int length = Math.Abs(delta);
      
      if (delta < 0)
        // newGap.Start < gap.Start < newGap.End < gap.End
        // Move delta chars before old gap to after old gap
        Copy(newGap.Start, newGap.End, length); 
      else if (delta > 0)
        // gap.Start < newGap.Start < gap.End < newGap.End
        // Move delta chars from end of old gap to before old gap
        Copy(gap.End, gap.Start, length);

      gap = newGap;
    }

    /// <summary>
    /// Copies a range of elements in the buffer.
    /// </summary>
    /// <param name="sourceIndex">Buffer index to copy from.</param>
    /// <param name="destinationIndex">Buffer index to copy to.</param>
    /// <param name="length">Number of items to copy.</param>
    void Copy(int sourceIndex, int destinationIndex, int length)
    {
      Array.Copy(buffer, sourceIndex, buffer, destinationIndex, length);
    }
  }
}
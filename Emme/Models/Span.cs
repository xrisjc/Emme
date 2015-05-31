//
// File: Span.cs
//
// Copyright (C) 2010 - 2013  Christopher Cowan
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

namespace Emme.Models
{
  public struct Span
  {
    readonly int start;
    readonly int end;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="start">Index in buffer where the gap starts and content ends.</param>
    /// <param name="end">Index in buffer where the gap ends and the rest of the content begins.</param>
    public Span(int start, int end)
    {
      this.start = start;
      this.end = end;
    }

    /// <summary>
    /// Index in buffer where the gap starts and content ends.
    /// </summary>
    public int Start
    {
      get { return start; }
    }

    /// <summary>
    /// Index in buffer where the gap ends and the rest of the content
    /// begins.
    /// </summary>
    public int End
    {
      get { return end; }
    }

    /// <summary>
    /// Length of the gap; the total available space left in buffer.
    /// </summary>
    public int Length
    {
      get { return End - Start; }
    }

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
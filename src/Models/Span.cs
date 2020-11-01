//
// File: Span.cs
//
// Copyright (C) 2010 - 2015  Christopher Cowan
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
    /// <summary>
    /// Represents a sequence of integers.
    /// </summary>
    public struct Span
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Start of Span, inclusive.</param>
        /// <param name="end">End of the Span, exclusive.</param>
        public Span(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Start of Span, inclusive.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// End of the Span, exclusive.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Length of the gap; the total available space left in buffer.
        /// </summary>
        public int Length => End - Start;
        
        /// <summary>
        /// Creates a new Span by moving Start a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>Span with new Start.</returns>
        public Span MoveStart(int delta) => new Span(Start + delta, End);

        /// <summary>
        /// Creates a new Span by moving End a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>Span with new End.</returns>
        public Span MoveEnd(int delta) => new Span(Start, End + delta);

        /// <summary>
        /// Creates a new Span by moving Start and End by a given amount.
        /// </summary>
        /// <param name="delta">Amount to move. May be negative.</param>
        /// <returns>Span with new Start and End.</returns>
        public Span Move(int delta) => new Span(Start + delta, End + delta);

        /// <summary>
        /// Override ToString for debugging purposes.
        /// </summary>
        public override string ToString() => $"Start = {Start}, End = {End}";
    }
}

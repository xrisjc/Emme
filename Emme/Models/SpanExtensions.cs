//
// File: SpanExtensions.cs
//
// Copyright (C) 2013  Christopher Cowan
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
using System.Diagnostics;
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
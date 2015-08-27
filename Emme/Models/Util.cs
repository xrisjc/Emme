//
// File: Util.cs
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

using static System.Math;

namespace Emme.Models
{
  /// <summary>
  /// Misc. utility methods.
  /// </summary>
  static class Util
  {
    /// <summary>
    /// Returns whether or not value is within the interval [start, end).
    /// </summary>
    public static bool IsInRange(this int value, int start, int end)
      => start <= value && value < end;

    public static int Clamp(this int value, int min, int max)
      => Max(min, Min(value, max));
  }
}

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

using System;

namespace Emme.Models
{
  /// <summary>
  /// Holds line, column content position.
  /// </summary>
  /// <remarks>
  /// Both Line and Column will start from zero to avoid subtracting one
  /// everywhere. But in the UI they may be shown to start at 1.
  /// </remarks>
  public struct Position : IEquatable<Position>
  {

    /// <summary>
    /// Line number, starting from zero
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Column number, starting from zero
    /// </summary>
    public int Column { get; }

    public Position(int line, int column)
    {
      Line = line;
      Column = column;
    }

    public int NextLine => Line + 1;

    public int PreviousColumn => Column - 1;

    public int NextColumn => Column + 1;

    /// <summary>
    /// Override ToString for debugging purposes.
    /// </summary>
    public override string ToString() => $"Line = {Line}, Column = {Column}";

    public bool Equals(Position other) => Line == other.Line && Column == other.Column;

    public override bool Equals(object obj)
    {
      if (obj is Position)
      {
        return Equals((Position)obj);
      }
      return base.Equals(obj);
    }

    public override int GetHashCode() => Line ^ Column;

    public static bool operator ==(Position lhs, Position rhs) => lhs.Equals(rhs);

    public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);

    public static Position operator +(Position lhs, Position rhs) =>
      new Position(lhs.Line + rhs.Line, lhs.Column + rhs.Column);

    public static Position operator -(Position lhs, Position rhs) =>
      new Position(lhs.Line - rhs.Line, lhs.Column - rhs.Column);

    public static readonly Position BufferStart = new Position(0, 0);

    public static readonly Position OneLine = new Position(1, 0);

    public static readonly Position OneColumn = new Position(0, 1);

  }
}
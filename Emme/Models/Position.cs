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
using System.Diagnostics;

namespace Emme.Models
{
  /// <summary>
  /// Holds line, column content position.
  /// </summary>
  /// <remarks>
  /// Both Line and Column will start from zero to avoid subtracting one
  /// everywhere. But in the UI they may be shown to start at 1.
  /// </remarks>
  struct Position : IEquatable<Position>
  {
    public Position(int line, int column)
    {
      Debug.Assert(line >= 0);
      Debug.Assert(column >= 0);

      Line = line;
      Column = column;
    }

    /// <summary>
    /// Line number, starting from zero
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Column number, starting from zero
    /// </summary>
    public int Column { get; }

    public Position SetColumn(int newColumn) => new Position(Line, newColumn);

    public Position SetLine(int newLine) => new Position(newLine, Column);

    public Position MoveColumn(Direction direction) => SetColumn(Column + direction.Delta());

    public Position MoveLine(Direction direction) => SetLine(Line + direction.Delta());

    public Position ClampColumn(int maxColumn)
    {
      int newColumn = Math.Min(maxColumn, Column);
      return new Position(Line, newColumn);
    }

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

    public static Position BufferStart => new Position(0, 0);
  }
}
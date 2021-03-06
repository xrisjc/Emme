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
    /// Holds line, column content position.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="line">Initial line number, zero-indexed.</param>
        /// <param name="column">Initial colum number, zero-indexed.</param>
        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Line number, zero-indexed.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Column number, zero-indexed.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Override ToString for debugging purposes.
        /// </summary>
        public override string ToString() => $"Line = {Line}, Column = {Column}";
    }
}

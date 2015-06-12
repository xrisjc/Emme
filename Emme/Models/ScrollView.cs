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

namespace Emme.Models
{
  struct ScrollView
  {
    public ScrollView(int topLine, int leftColumn, int lines, int columns)
    {
      TopLine = topLine;
      LeftColumn = leftColumn;
      Lines = lines;
      Columns = columns;
    }

    public int TopLine { get; }

    public int LeftColumn { get; }

    public int Lines { get; }

    public int Columns { get; }

    public int BottomLine => TopLine + Lines;

    public ScrollView LineUp() => new ScrollView(TopLine - 1, LeftColumn, Lines, Columns);

    public ScrollView LineDown() => new ScrollView(TopLine + 1, LeftColumn, Lines, Columns);

    public Position PositionInView(Position positionInFile)
    {
      return new Position(positionInFile.Line - TopLine, positionInFile.Column - LeftColumn);
    }
  }
}

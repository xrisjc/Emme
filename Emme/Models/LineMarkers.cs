//
// File: LineMarkers.cs
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
    public class LineMarkers
    {
        private GapBuffer<Span> Markers { get; }

        public LineMarkers(GapBuffer<Span> markers)
        {
            Markers = markers;
        }

        private void Move(int line, int delta)
        {
            int start = line;
            Markers[start] = Markers[start].MoveEnd(delta);
            for (int i = start + 1; i < Markers.Count; i++)
            {
                Markers[i] = Markers[i].Move(delta);
            }
        }

        public int LineCount => Markers.Count;

        public int Start(int line) => Markers[line].Start;

        public int Start(Position position) => Start(position.Line);

        public int End(int line) => Markers[line].End;

        public int End(Position position) => End(position.Line);

        public int Length(int line) => Markers[line].Length;

        public int Length(Position position) => Length(position.Line);

        public int BufferIndex(Position position) =>
            Markers[position.Line].Start + position.Column;

        public void Insert(Position position) => Move(position.Line, 1);

        public void Delete(Position position) => Move(position.Line, -1);

        public void Split(Position position)
        {
            Span oldLine = Markers[position.Line];
            var newFirstLine = new Span(oldLine.Start, BufferIndex(position));
            var newSecondLine = new Span(BufferIndex(position), oldLine.End);
            Markers[position.Line] = newFirstLine;
            Markers.Insert(position.Line + 1, newSecondLine);
        }

        public void Join(Position position)
        {
            var newLine = new Span(
              Markers[position.Line].Start,
              Markers[position.Line + 1].End);
            Markers[position.Line] = newLine;
            Markers.Delete(position.Line + 1);
        }
    }
}

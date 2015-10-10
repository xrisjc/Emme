//
// File: TextView.cs
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
using System.Collections.Generic;
using System.Text;
using Emme.Models;
using static System.Math;

namespace Emme.Editing
{
    public class TextView
    {
        public GapBuffer<char> GapBuffer { get; }
        public LineMarkers LineMarkers { get; }
        public Position Caret { get; set; } = Position.BufferStart;
        public ScrollView ScrollView { get; set; } = new ScrollView(lineStart: 0, columnStart: 0, lines: 24, columns: 80);
        public int? DesiredColumn { get; set; } = null;

        /// <summary>
        /// Primary constructor
        /// </summary>
        public TextView(GapBuffer<char> gapBuffer, LineMarkers lineMarkers)
        {
            GapBuffer = gapBuffer;
            LineMarkers = lineMarkers;
        }

        /// <summary>
        /// The number of lines this TextView contains.
        /// </summary>
        public int LastLine => LineMarkers.LineCount - 1;

        /// <summary>
        /// The index of the caret in the text buffer.
        /// </summary>
        public int CaretBufferIndex => LineMarkers.BufferIndex(Caret);

        public void ResizeScrollView(int lines, int columns)
        {
            ScrollView.Resize(lines, columns);
        }

        public void GapBufferInsert(char valueToInsert)
        {
            GapBuffer.Insert(CaretBufferIndex, valueToInsert);
            LineMarkers.Insert(Caret);
        }

        public char GapBufferDelete()
        {
            char deletedValue = GapBuffer[CaretBufferIndex];
            GapBuffer.Delete(CaretBufferIndex);
            LineMarkers.Delete(Caret);
            return deletedValue;
        }

        public void MoveCaretToLine(int line)
        {
            DesiredColumn = DesiredColumn ?? Caret.Column;
            int nextColumn = Min(LineMarkers.Length(line), DesiredColumn.Value);
            Caret = new Position(line, nextColumn);
        }

        public void CheckVerticalScroll()
        {
            ScrollView.CheckVerticalScroll(Caret);
        }

        public void CheckHorizontalScroll()
        {
            ScrollView.CheckHorizontalScroll(Caret);
        }

        public void CheckScroll()
        {
            ScrollView.CheckVerticalScroll(Caret);
            ScrollView.CheckHorizontalScroll(Caret);
        }

        public void PageUpScroll()
        {
            ScrollView.CheckPageUp(Caret);
            CheckHorizontalScroll();
        }

        public void PageDownScroll()
        {
            ScrollView.CheckPageDown(Caret);
            CheckHorizontalScroll();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var line = 0; line < LineMarkers.LineCount; line++)
            {
                for (int i = LineMarkers.Start(line); i < LineMarkers.End(line); i++)
                {
                    sb.Append(GapBuffer[i]);
                }
                if (line + 1 < LineMarkers.LineCount)
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public IEnumerable<string> EnumerateLines()
        {
            var sb = new StringBuilder();
            int lineStart = ScrollView.LineStart;
            int lineEnd = Min(lineStart + ScrollView.Lines, LineMarkers.LineCount);
            for (var line = lineStart; line < lineEnd; line++)
            {
                sb.Clear();
                int iStart = LineMarkers.Start(line) + ScrollView.ColumnStart;
                int iEnd = Min(iStart + ScrollView.Columns, LineMarkers.End(line));
                for (int i = iStart; i < iEnd; i++)
                {
                    sb.Append(GapBuffer[i]);
                }
                yield return sb.ToString();
            }
        }
    }
}

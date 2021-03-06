﻿//
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

using Emme.Models;

namespace Emme.Editing
{
    public class EditCommandWordRight : IEditCommand
    {
        public IEditCommand Execute(TextView textView)
        {
            textView.DesiredColumn = null;
            if (textView.Caret.Column < textView.LineMarkers.Length(textView.Caret))
            {
                // Caret is not at the end of the line.
                int iStart = textView.CaretBufferIndex;
                int iMax = textView.LineMarkers.End(textView.Caret);
                int i = iStart;
                while (i < iMax && !char.IsWhiteSpace(textView.GapBuffer[i]))
                {
                    i++;
                }
                while (i < iMax && char.IsWhiteSpace(textView.GapBuffer[i]))
                {
                    i++;
                }
                textView.Caret = new Position(
                    textView.Caret.Line,
                    textView.Caret.Column + i - iStart);
            }
            else if (textView.Caret.Line < textView.LastLine)
            {
                // At the end of the line, and it's not the last line.
                textView.Caret = new Position(textView.Caret.Line + 1, column: 0);
            }
            textView.CheckScroll();
            return EditCommand.NoOp();
        }

        public override string ToString() => "WordRight";
    }
}

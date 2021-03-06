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
using static Emme.Editing.EditCommand;

namespace Emme.Editing
{
    public class EditCommandDeleteBackwards : IEditCommand
    {
        public IEditCommand Execute(TextView textView)
        {
            if (textView.Caret.Column > 0)
            {
                // In the middle of a line, and not at the start.
                textView.Caret = new Position(textView.Caret.Line, textView.Caret.Column - 1);
                char deletedChar = textView.GapBufferDelete();
                textView.CheckHorizontalScroll();
                return SetCaret(textView.Caret).Then(Insert(deletedChar));
            }
            else if (textView.Caret.Line > 0)
            {
                // At the beginning of a line but not at first line.
                textView.Caret = new Position(
                    textView.Caret.Line - 1,
                    textView.LineMarkers.Length(textView.Caret.Line - 1));
                textView.CheckScroll();
                return Do<EditCommandJoinLines>().Execute(textView);
            }
            else
            {
                return NoOp();
            }
        }

        public override string ToString() => "DeleteBackwards";
    }
}


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

using static Emme.Editing.EditCommand;

namespace Emme.Editing
{
    public class EditCommandDelete : IEditCommand
    {
        public IEditCommand Execute(TextView textView)
        {
            if (textView.Caret.Column < textView.LineMarkers.Length(textView.Caret))
            {
                char charUndo = textView.GapBufferDelete();
                return SetCaret(textView.Caret).Then(Insert(charUndo)).Then<EditCommandCharLeft>();
            }
            else if (textView.Caret.NextLine < textView.LineMarkers.LineCount)
            {
                return Execute<EditCommandJoinLines>(textView);
            }
            else
            {
                return NoOp();
            }
        }

        public override string ToString() => "Delete";
    }
}

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

using Emme.Models;

namespace Emme.Editing
{
  public class EditCommandDeleteBackwards : IEditCommand
  {
    public IEditCommand Execute(TextView textView)
    {
      if (textView.Caret.Column > 0)
      {
        // In the middle of a line, and not at the start.
        textView.Caret -= Position.OneColumn;

        textView.GapBuffer.Delete(textView.CaretBufferIndex);

        textView.ShiftLines(-1);

        textView.ScrollView.CheckHorizontalScroll(textView.Caret);
      }
      else if (textView.Caret.Line > 0)
      {
        // At the beginning of a line but not at first line.
        textView.Caret = new Position(textView.Caret.PreviousLine, textView.Lines[textView.Caret.PreviousLine].Length);

        textView.Lines[textView.Caret.Line] =
          textView.Lines[textView.Caret.Line].Join(
            textView.Lines[textView.Caret.NextLine]);
        textView.Lines.Delete(textView.Caret.NextLine);

        textView.ScrollView.CheckLineUp(textView.Caret)
                  .CheckHorizontalScroll(textView.Caret);
      }

      return new EditCommandNoOp();
    }
  }
}


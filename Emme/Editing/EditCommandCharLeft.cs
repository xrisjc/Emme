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
  public class EditCommandCharLeft : IEditCommand
  {
    public IEditCommand Execute(TextView textView)
    {
      textView.DesiredColumn = null;
      if (textView.Caret.Column > 0)
      {
        textView.Caret -= Position.OneColumn;
      }
      else if (textView.Caret.Line > 0)
      {
        textView.Caret = new Position(textView.Caret.PreviousLine, column: textView.Lines[textView.Caret.PreviousLine].Length);
      }
      textView.ScrollView.CheckLineUp(textView.Caret).CheckHorizontalScroll(textView.Caret);

      return EditCommand.NoOp();
    }
  }
}

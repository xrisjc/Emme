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
  public class EditCommandSplitLines : IEditCommand
  {
    public IEditCommand Execute(TextView textView)
    {
      Span oldLine = textView.Lines[textView.Caret.Line];
      var newFirstLine = new Span(oldLine.Start, textView.CaretBufferIndex);
      var newSecondLine = new Span(textView.CaretBufferIndex, oldLine.End);
      textView.Lines[textView.Caret.Line] = newFirstLine;
      textView.Lines.Insert(textView.Caret.NextLine, newSecondLine);

      return EditCommand.NoOp();
    }
  }
}
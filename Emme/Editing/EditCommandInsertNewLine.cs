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
using System;

namespace Emme.Editing
{
  public class EditCommandInsertNewLine : IEditCommand
  {
    public void Execute(TextView textView)
    {
      textView.DesiredColumn = null;
      Tuple<Span, Span> splitSpans = textView.Lines[textView.CaretPosition.Line].Split(textView.CaretBufferIndex);
      textView.Lines[textView.CaretPosition.Line] = splitSpans.Item1;
      textView.CaretPosition = new Position(textView.CaretPosition.NextLine, column: 0);
      textView.Lines.Insert(textView.CaretPosition.Line, splitSpans.Item2);
      textView.ScrollView.CheckLineDown(textView.CaretPosition)
                .CheckHorizontalScroll(textView.CaretPosition);
    }
  }
}

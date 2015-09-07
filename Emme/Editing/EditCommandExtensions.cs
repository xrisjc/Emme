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

using System.Collections.Generic;

namespace Emme.Editing
{
  public static class EditCommandExtensions
  {
    public static void Execute(this IEditCommand editCommand, TextView textView,Stack<IEditCommand> undo)
    {
      IEditCommand undoCommand = editCommand.Execute(textView);
      if (undoCommand is EditCommandNoOp == false)
      {
        undo.Push(undoCommand);
      }
    }

    public static void Execute(this IEditCommand editCommand, TextView textView,
      Stack<IEditCommand> undo, Stack<IEditCommand> redo)
    {
      editCommand.Execute(textView, undo);
      redo.Clear();
    }

    public static void Undo(this Stack<IEditCommand> undo, TextView textView, Stack<IEditCommand> redo)
    {
      if (undo.Count > 0)
      {
        undo.Pop().Execute(textView, redo);
      }
    }
  }
}

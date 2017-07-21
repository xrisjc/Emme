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
using System.Collections.Generic;

namespace Emme.Editing
{
    /// <summary>
    /// Static methods for handling IEditCommand functionality.
    /// </summary>
    public static class EditCommand
    {
        public static void Execute(this IEditCommand editCommand, TextView textView, Stack<IEditCommand> undo)
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

        public static IEditCommand Do<T>()
          where T : IEditCommand, new()
        {
            return new T();
        }

        public static IEditCommand Then(this IEditCommand firstEditCommand, IEditCommand secondEditCommand)
        {
            if (firstEditCommand is EditCommandNoOp)
            {
                return secondEditCommand;
            }
            else if (secondEditCommand is EditCommandNoOp)
            {
                return firstEditCommand;
            }
            else
            {
                return new EditCommandChain(firstEditCommand, secondEditCommand);
            }
        }

        public static IEditCommand Then<T>(this IEditCommand firstEditCommand)
          where T : IEditCommand, new()
        {
            return firstEditCommand.Then(new T());
        }

        public static IEditCommand Insert(char value)
        {
            return new EditCommandInsert(value);
        }

        public static IEditCommand SetCaret(Position caret)
        {
            return new EditCommandSetCaret(caret);
        }

        public static IEditCommand NoOp()
        {
            return new EditCommandNoOp();
        }
    }
}

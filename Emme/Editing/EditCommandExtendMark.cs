using Emme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Editing
{
    public class EditCommandExtendMark : IEditCommand
    {
        public IEditCommand MovementComand { get; }

        public EditCommandExtendMark(IEditCommand movementComand)
        {
            MovementComand = movementComand;
        }

        public IEditCommand Execute(TextView textView)
        {
            Position mark = textView.Mark ?? textView.Caret;
            MovementComand.Execute(textView);
            if (mark.Line == textView.Caret.Line &&
                mark.Column == textView.Caret.Column)
            {
                textView.Mark = mark;
            }
            return EditCommand.NoOp();
        }
    }
}

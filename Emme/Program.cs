//
// Copyright (C) 2010  Christopher Cowan
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

//
// "Entities must not be multiplied beyond necessity."
//                                      -- Occam's razor
//
// "Perfection is achieved, not when there is nothing more to add, but when
//  there is nothing left to take away."
//                                      -- Antoine de Saint Exupéry
//
// "See with original purity
//  Embrace with original simmplicity
//  Reduce what you have
//  Decrease what you want"
//                                      -- Tao Te Ching
//
// "Brevity is the soul of wit."
//                                      -- Shakespeare
//
// "A witty saying proves nothing."
//                                      -- Voltaire
//

using System.Windows.Forms;
using Emme.UI;
using Emme.Editing;
using Emme.Models;

namespace Emme
{
  public class Program
  {
    public static void Main()
    {
      var buffer = new GapBuffer();
      var textView = new TextView(buffer);

      Application.EnableVisualStyles();
      using (var editor = new EditorForm(textView))
      {
        Application.Run(editor);
      }
    }
  }
}
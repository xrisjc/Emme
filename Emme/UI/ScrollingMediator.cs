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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emme.UI
{
  class ScrollingMediator
  {
    private readonly VScrollBar vScrollBar;
    private readonly HScrollBar hScrollBar;

    public ScrollingMediator(Control scrollingControl, HScrollBar hScrollBar, VScrollBar vScrollBar)
    {
      this.vScrollBar = vScrollBar;
      this.hScrollBar = hScrollBar;
      scrollingControl.Controls.Add(hScrollBar);
      scrollingControl.Controls.Add(vScrollBar);

      scrollingControl.Resize += ScrollingControl_Resize;
    }

    private void ScrollingControl_Resize(object sender, EventArgs e)
    {
      var control = sender as Control;
      AdjustScrollBarPositions(control.DisplayRectangle);
    }

    private void AdjustScrollBarPositions(Rectangle bounds)
    {
      hScrollBar.Location = new Point(0, bounds.Height - hScrollBar.Height);
      vScrollBar.Location = new Point(bounds.Width - vScrollBar.Width, 0);
      hScrollBar.Width = bounds.Width - vScrollBar.Width;
      vScrollBar.Height = bounds.Height - hScrollBar.Height;
    }
  }
}

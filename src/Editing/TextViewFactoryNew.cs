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
    public class TextViewFactoryNew : ITextViewFactory
    {
        public TextView Create()
        {
            var content = new GapBuffer<char>();
            var lines = new GapBuffer<Span>().Insert(0, new Span(0, 0));
            var lineMarkers = new LineMarkers(lines);
            return new TextView(content, lineMarkers);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Models
{
    public class LineMarkers
    {
        public int BufferIndex(Position position)
        {
            return position.Column;
        }

        public void Insert(Position position)
        {
                        
        }
    }
}
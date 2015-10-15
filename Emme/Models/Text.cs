using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Models
{
    public class Text
    {
        public Position Position { get; }

        public string Content { get; }

        public bool IsHighlighted { get; }

        public Text(Position position, string content, bool isHighlighted)
        {
            Position = position;
            Content = content;
            IsHighlighted = isHighlighted;
        }
    }
}

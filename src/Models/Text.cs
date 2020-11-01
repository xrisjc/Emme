using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emme.Models
{
    public class Text
    {
        public string Content { get; }

        public bool IsHighlighted { get; }

        public Text(string content, bool isHighlighted)
        {
            Content = content;
            IsHighlighted = isHighlighted;
        }
    }
}

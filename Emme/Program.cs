using System;
using System.Windows.Forms;
using Emme.UI;

namespace Emme
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            using (var editor = new EditorForm())
            {
                Application.Run(editor);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hirc
{
    class Program
    {
        public static string StartUpDirectory;
        [STAThread]
        public static void Main(string[] args)
        {
            StartUpDirectory = Application.StartupPath;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormShell());
        }
    }
}

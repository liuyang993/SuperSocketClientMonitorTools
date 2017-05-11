using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperSocketClientTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //Action<string> messageTarget;
            //messageTarget = ShowWindowsMessage;

            //messageTarget("Hello, World!");
        }
        private static void ShowWindowsMessage(string message)
        {
            MessageBox.Show(message);
        }

    }
}

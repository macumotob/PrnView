using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrnView
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        [MTAThread]
        static void Main()
        {
            Task.Run( ()=> new PrnServer(3044));

            while (true)
            {
                Thread.Sleep(5000);
            }
            //Utils.TestRenderLine();
            //return;
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}

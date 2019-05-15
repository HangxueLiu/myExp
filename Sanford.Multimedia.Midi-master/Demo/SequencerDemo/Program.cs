using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SequencerDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]//是一种线程模型，用在程序的入口方法上
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var mainWindow = new Form1();
                if (args.Length >= 1)
                {
                    mainWindow.Open(args[0]);
                }
                Application.Run(mainWindow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
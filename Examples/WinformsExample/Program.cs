// SPDX-License-Identifier: MIT

using System;
using System.Windows.Forms;

namespace WinformsExample
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

            var mainCaptureForm = new CaptureForm();
            Application.Run(mainCaptureForm);
        }
    }
}

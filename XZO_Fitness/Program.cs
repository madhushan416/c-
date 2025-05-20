using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XZO_Fitness
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the login form first
            //login loginForm = new login();
            //if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Navigate to Form1 after successful login
                Application.Run(new Form1());
            }

        }
    }
}

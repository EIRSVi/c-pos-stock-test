using System;
using System.Windows.Forms;

namespace Stock_Room
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Start with the login form
            Application.Run(new LoginForm());
        }
    }
}
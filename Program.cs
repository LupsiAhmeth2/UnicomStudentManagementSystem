using System;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;
using UnicomTICManagementSystem.Views; // ✅ Add this to access LoginForm

namespace UnicomTICManagementSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ✅ Step 1: Initialize the database before opening the UI
            DatabaseManager.InitializeDatabase();

            // ✅ Step 2: Load the Login UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm()); // ✅ This shows your login form at startup
        }
    }
}

using System;
using System.Windows.Forms;
using EmployeeSalaryManagement.Data;
using EmployeeSalaryManagement.Forms;

namespace EmployeeSalaryManagement
{
    /// <summary>
    /// Application entry point.
    /// Initializes SQLite database and launches the Login Form.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Ensure SQLite database and tables exist with seed data
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error initializing the database:\n\n" + ex.Message +
                    "\n\nPlease ensure you have write permissions in the application folder.",
                    "Database Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Launch the application through the Login Form
            Application.Run(new LoginForm());
        }
    }
}

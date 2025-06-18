using System;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            cmbRole.Items.Clear();
            cmbRole.Items.AddRange(new object[] { "Admin", "Staff", "Lecturer", "Student" });
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string selectedRole = cmbRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Please enter username, password and select a role.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password AND Role = @role";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", selectedRole);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            MessageBox.Show($"Login successful! Welcome {selectedRole}.");
                            this.Hide();

                            switch (selectedRole)
                            {
                                case "Admin": new MainForm("Admin").Show(); break;
                                case "Staff": new StaffDashboard().Show(); break;
                                case "Lecturer": new LecturerDashboard().Show(); break;
                                case "Student": new StudentDashboard(username).Show(); break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid credentials.");
                        }
                    }
                }
            }
        }
    }
}

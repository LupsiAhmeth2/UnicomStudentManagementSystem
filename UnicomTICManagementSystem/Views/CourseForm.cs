using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class CourseForm : Form
    {
        public CourseForm()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void LoadCourses()
        {
            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Courses";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCourses.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtCourseID.Text = "";
            txtCourseName.Text = "";
        }

        // ✅ Add button
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string courseName = txtCourseName.Text.Trim();

            if (string.IsNullOrEmpty(courseName))
            {
                MessageBox.Show("Please enter a course name.");
                return;
            }

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Courses (CourseName) VALUES (@name)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", courseName);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Course added successfully.");
            LoadCourses();
            ClearInputs();
        }

        // ✅ Update button renamed to match actual name: button1
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCourseID.Text))
            {
                MessageBox.Show("Please select a course to update.");
                return;
            }

            string courseName = txtCourseName.Text.Trim();
            if (string.IsNullOrEmpty(courseName))
            {
                MessageBox.Show("Course name cannot be empty.");
                return;
            }

            using (var connection = DatabaseManager.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Courses SET CourseName = @name WHERE CourseID = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", courseName);
                    cmd.Parameters.AddWithValue("@id", txtCourseID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Course updated successfully.");
            LoadCourses();
            ClearInputs();
        }

        // ✅ Delete button renamed to match actual name: button2
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCourseID.Text))
            {
                MessageBox.Show("Please select a course to delete.");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this course?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM Courses WHERE CourseID = @id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCourseID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Course deleted successfully.");
                LoadCourses();
                ClearInputs();
            }
        }

       

        // ✅ When row is clicked, fill form
        private void dgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCourses.Rows[e.RowIndex];
                txtCourseID.Text = row.Cells["CourseID"].Value.ToString();
                txtCourseName.Text = row.Cells["CourseName"].Value.ToString();
            }
        }

        // ❌ Unused/empty auto-generated events — safe to delete
        private void btnClear_Click_1(object sender, EventArgs e) {
            ClearInputs();
        }
    }
}

using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
            LoadCourses();
            LoadStudents();
        }

        private void LoadCourses()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT CourseID, CourseName FROM Courses";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                cmbCourse.DisplayMember = "CourseName";
                cmbCourse.ValueMember = "CourseID";
                cmbCourse.DataSource = dt;
            }
        }

        private void LoadStudents()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT s.StudentID, s.Name, c.CourseName FROM Students s JOIN Courses c ON s.CourseID = c.CourseID";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvStudents.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtStudentID.Text = "";
            txtName.Text = "";
            cmbCourse.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name) || cmbCourse.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter name and select course.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Students (Name, CourseID) VALUES (@name, @courseId)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Student added.");
            LoadStudents();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentID.Text))
            {
                MessageBox.Show("Please select a student to update.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Students SET Name = @name, CourseID = @courseId WHERE StudentID = @id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", txtStudentID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Student updated.");
            LoadStudents();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentID.Text))
            {
                MessageBox.Show("Please select a student to delete.");
                return;
            }

            if (MessageBox.Show("Confirm delete?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Students WHERE StudentID = @id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtStudentID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Student deleted.");
                LoadStudents();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];
                txtStudentID.Text = row.Cells["StudentID"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                cmbCourse.Text = row.Cells["CourseName"].Value.ToString();
            }
        }
    }
}

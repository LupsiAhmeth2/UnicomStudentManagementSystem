using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class SubjectForm : Form
    {
        public SubjectForm()
        {
            InitializeComponent();
            LoadCourses();
            LoadSubjects();
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

        private void LoadSubjects()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.SubjectID, s.SubjectName, c.CourseName 
                                 FROM Subjects s 
                                 JOIN Courses c ON s.CourseID = c.CourseID";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvSubjects.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtSubjectID.Text = "";
            txtSubjectName.Text = "";
            cmbCourse.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjectName.Text) || cmbCourse.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter subject name and select course.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Subjects (SubjectName, CourseID) VALUES (@name, @courseId)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtSubjectName.Text.Trim());
                    cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Subject added.");
            LoadSubjects();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjectID.Text)) return;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Subjects SET SubjectName = @name, CourseID = @courseId WHERE SubjectID = @id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtSubjectName.Text.Trim());
                    cmd.Parameters.AddWithValue("@courseId", cmbCourse.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", txtSubjectID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Subject updated.");
            LoadSubjects();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjectID.Text)) return;

            DialogResult confirm = MessageBox.Show("Are you sure to delete this subject?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Subjects WHERE SubjectID = @id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtSubjectID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Subject deleted.");
                LoadSubjects();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSubjects.Rows[e.RowIndex];
                txtSubjectID.Text = row.Cells["SubjectID"].Value.ToString();
                txtSubjectName.Text = row.Cells["SubjectName"].Value.ToString();
                cmbCourse.Text = row.Cells["CourseName"].Value.ToString(); // uses display value
            }
        }
    }
}

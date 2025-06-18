using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class ExamForm : Form
    {
        public ExamForm()
        {
            InitializeComponent();
            LoadSubjects();
            LoadExams();
        }

        private void LoadSubjects()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT SubjectID, SubjectName FROM Subjects";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                cmbSubject.DisplayMember = "SubjectName";
                cmbSubject.ValueMember = "SubjectID";
                cmbSubject.DataSource = dt;
            }
        }

        private void LoadExams()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT e.ExamID, e.ExamName, s.SubjectName FROM Exams e JOIN Subjects s ON e.SubjectID = s.SubjectID";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvExams.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtExamID.Text = "";
            txtExamName.Text = "";
            cmbSubject.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string examName = txtExamName.Text.Trim();
            if (string.IsNullOrEmpty(examName) || cmbSubject.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter exam name and select subject.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Exams (ExamName, SubjectID) VALUES (@name, @subjectId)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", examName);
                    cmd.Parameters.AddWithValue("@subjectId", cmbSubject.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Exam added successfully.");
            LoadExams();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtExamID.Text)) return;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Exams SET ExamName = @name, SubjectID = @subjectId WHERE ExamID = @id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtExamName.Text.Trim());
                    cmd.Parameters.AddWithValue("@subjectId", cmbSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", txtExamID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Exam updated successfully.");
            LoadExams();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtExamID.Text)) return;

            DialogResult result = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Exams WHERE ExamID = @id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtExamID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Exam deleted successfully.");
                LoadExams();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvExams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvExams.Rows[e.RowIndex];
                txtExamID.Text = row.Cells["ExamID"].Value.ToString();
                txtExamName.Text = row.Cells["ExamName"].Value.ToString();
                cmbSubject.Text = row.Cells["SubjectName"].Value.ToString();
            }
        }
    }
}

using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class MarkForm : Form
    {
        public MarkForm()
        {
            InitializeComponent();
            LoadStudents();
            LoadSubjects();
            LoadMarks();
        }

        private void LoadStudents()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT StudentID, Name FROM Students";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbStudent.DataSource = dt;
                cmbStudent.DisplayMember = "Name";
                cmbStudent.ValueMember = "StudentID";
            }
        }

        private void LoadSubjects()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT SubjectID, SubjectName FROM Subjects";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbSubject.DataSource = dt;
                cmbSubject.DisplayMember = "SubjectName";
                cmbSubject.ValueMember = "SubjectID";
            }
        }

        private void LoadMarks()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT m.MarkID, s.Name AS StudentName, sub.SubjectName, m.Score
                    FROM Marks m
                    JOIN Students s ON m.StudentID = s.StudentID
                    JOIN Exams e ON m.ExamID = e.ExamID
                    JOIN Subjects sub ON e.SubjectID = sub.SubjectID";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvMarks.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtMarks.Text = "";
            cmbStudent.SelectedIndex = -1;
            cmbSubject.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                // Get ExamID for selected subject
                string getExamQuery = "SELECT ExamID FROM Exams WHERE SubjectID = @subjectID LIMIT 1";
                SQLiteCommand getExamCmd = new SQLiteCommand(getExamQuery, conn);
                getExamCmd.Parameters.AddWithValue("@subjectID", cmbSubject.SelectedValue);
                object examIdObj = getExamCmd.ExecuteScalar();

                if (examIdObj == null)
                {
                    MessageBox.Show("No exam found for selected subject.");
                    return;
                }

                int examId = Convert.ToInt32(examIdObj);

                string insertQuery = "INSERT INTO Marks (StudentID, ExamID, Score) VALUES (@studentID, @examID, @score)";
                SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@studentID", cmbStudent.SelectedValue);
                insertCmd.Parameters.AddWithValue("@examID", examId);
                insertCmd.Parameters.AddWithValue("@score", txtMarks.Text);
                insertCmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mark added.");
            LoadMarks();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMarkID.Text)) return;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                string getExamQuery = "SELECT ExamID FROM Exams WHERE SubjectID = @subjectID LIMIT 1";
                SQLiteCommand getExamCmd = new SQLiteCommand(getExamQuery, conn);
                getExamCmd.Parameters.AddWithValue("@subjectID", cmbSubject.SelectedValue);
                object examIdObj = getExamCmd.ExecuteScalar();

                if (examIdObj == null)
                {
                    MessageBox.Show("No exam found for selected subject.");
                    return;
                }

                int examId = Convert.ToInt32(examIdObj);

                string query = "UPDATE Marks SET StudentID = @student, ExamID = @exam, Score = @score WHERE MarkID = @id";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@student", cmbStudent.SelectedValue);
                cmd.Parameters.AddWithValue("@exam", examId);
                cmd.Parameters.AddWithValue("@score", txtMarks.Text);
                cmd.Parameters.AddWithValue("@id", txtMarkID.Text);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mark updated.");
            LoadMarks();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMarkID.Text)) return;

            if (MessageBox.Show("Confirm delete?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Marks WHERE MarkID = @id";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", txtMarkID.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Mark deleted.");
                LoadMarks();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvMarks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMarks.Rows[e.RowIndex];
                txtMarkID.Text = row.Cells["MarkID"].Value.ToString();
                cmbStudent.Text = row.Cells["StudentName"].Value.ToString();
                cmbSubject.Text = row.Cells["SubjectName"].Value.ToString();
                txtMarks.Text = row.Cells["Score"].Value.ToString();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

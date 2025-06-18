using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class StudentDashboard : Form
    {
        private int studentId;

        public StudentDashboard(string username)
        {
            InitializeComponent();
            studentId = GetStudentIdByUsername(username);
        }

        private int GetStudentIdByUsername(string username)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT StudentID FROM Students WHERE Username = @username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        private void btnViewTimetable_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT t.TimetableID, s.SubjectName, t.TimeSlot, r.RoomName
                    FROM Timetables t
                    JOIN Subjects s ON t.SubjectID = s.SubjectID
                    JOIN Rooms r ON t.RoomID = r.RoomID";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDisplay.DataSource = dt;
            }
        }

        private void btnViewMarks_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT m.MarkID, sub.SubjectName, m.Score
                    FROM Marks m
                    JOIN Exams e ON m.ExamID = e.ExamID
                    JOIN Subjects sub ON e.SubjectID = sub.SubjectID
                    WHERE m.StudentID = @studentId";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvDisplay.DataSource = dt;
                }
            }
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT s.SubjectName, a.Date, a.Status
                    FROM Attendance a
                    JOIN Subjects s ON a.SubjectID = s.SubjectID
                    WHERE a.StudentID = @studentId";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvDisplay.DataSource = dt;
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }
    }
}

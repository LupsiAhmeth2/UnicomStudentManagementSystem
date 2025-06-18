using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
            this.Load += AttendanceForm_Load;
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            InitializeAttendanceTable(); // Ensure columns exist
            LoadStudents();
            LoadSubjects();
            LoadAttendanceData();

            cmbStatus.Items.Clear(); // ✅ Prevent duplicates
            cmbStatus.Items.AddRange(new string[] { "Present", "Absent", "Late", "Excused" });
            cmbStatus.SelectedIndex = 0;
        }

        private void InitializeAttendanceTable()
        {
            if (dgvAttendance.Columns.Count == 0)
            {
                dgvAttendance.Columns.Add("AttendanceID", "Attendance ID");
                dgvAttendance.Columns.Add("StudentID", "Student ID");
                dgvAttendance.Columns.Add("SubjectID", "Subject ID");
                dgvAttendance.Columns.Add("Date", "Date");
                dgvAttendance.Columns.Add("Status", "Status");
            }
        }

        private void LoadStudents()
        {
            cmbStudentID.Items.Clear();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT StudentID FROM Students";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbStudentID.Items.Add(reader["StudentID"].ToString());
                    }
                }
            }
        }

        private void LoadSubjects()
        {
            cmbSubjectID.Items.Clear();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT SubjectID FROM Subjects";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbSubjectID.Items.Add(reader["SubjectID"].ToString());
                    }
                }
            }
        }

        private void LoadAttendanceData()
        {
            dgvAttendance.Rows.Clear();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Attendance";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgvAttendance.Rows.Add(
                            reader["AttendanceID"],
                            reader["StudentID"],
                            reader["SubjectID"],
                            reader["Date"],
                            reader["Status"]
                        );
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtAttendanceID.Clear();
            cmbStudentID.SelectedIndex = -1;
            cmbSubjectID.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            dtpDate.Value = DateTime.Now;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbStudentID.Text == "" || cmbSubjectID.Text == "")
            {
                MessageBox.Show("Please select Student and Subject.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string insert = "INSERT INTO Attendance (StudentID, SubjectID, Date, Status) VALUES (@sid, @subid, @date, @status)";
                using (var cmd = new SQLiteCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@sid", cmbStudentID.Text);
                    cmd.Parameters.AddWithValue("@subid", cmbSubjectID.Text);
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadAttendanceData();
            ClearForm();
            MessageBox.Show("Attendance added successfully.");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtAttendanceID.Text == "")
            {
                MessageBox.Show("Select a record to update.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string update = "UPDATE Attendance SET StudentID=@sid, SubjectID=@subid, Date=@date, Status=@status WHERE AttendanceID=@aid";
                using (var cmd = new SQLiteCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@sid", cmbStudentID.Text);
                    cmd.Parameters.AddWithValue("@subid", cmbSubjectID.Text);
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@aid", txtAttendanceID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadAttendanceData();
            ClearForm();
            MessageBox.Show("Attendance updated.");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtAttendanceID.Text == "")
            {
                MessageBox.Show("Select a record to delete.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure to delete?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string delete = "DELETE FROM Attendance WHERE AttendanceID=@aid";
                    using (var cmd = new SQLiteCommand(delete, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", txtAttendanceID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadAttendanceData();
                ClearForm();
                MessageBox.Show("Deleted.");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtAttendanceID.Text = dgvAttendance.Rows[e.RowIndex].Cells[0].Value.ToString();
                cmbStudentID.Text = dgvAttendance.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbSubjectID.Text = dgvAttendance.Rows[e.RowIndex].Cells[2].Value.ToString();
                dtpDate.Text = dgvAttendance.Rows[e.RowIndex].Cells[3].Value.ToString();
                cmbStatus.Text = dgvAttendance.Rows[e.RowIndex].Cells[4].Value.ToString();
            }
        }
    }
}

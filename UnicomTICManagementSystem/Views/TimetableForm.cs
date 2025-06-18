using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class TimetableForm : Form
    {
        public TimetableForm()
        {
            InitializeComponent();
            LoadSubjects();
            LoadRooms();
            LoadTimetables();
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
                cmbSubject.DisplayMember = "SubjectName";
                cmbSubject.ValueMember = "SubjectID";
                cmbSubject.DataSource = dt;
            }
        }

        private void LoadRooms()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT RoomID, RoomName FROM Rooms";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbRoom.DisplayMember = "RoomName";
                cmbRoom.ValueMember = "RoomID";
                cmbRoom.DataSource = dt;
            }
        }

        private void LoadTimetables()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT 
                        t.TimetableID,
                        s.SubjectName,
                        t.TimeSlot,
                        r.RoomName
                    FROM Timetables t
                    JOIN Subjects s ON t.SubjectID = s.SubjectID
                    JOIN Rooms r ON t.RoomID = r.RoomID";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvTimetables.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtTimetableID.Text = "";
            txtTimeSlot.Text = "";
            cmbSubject.SelectedIndex = -1;
            cmbRoom.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbSubject.SelectedIndex == -1 || cmbRoom.SelectedIndex == -1 || string.IsNullOrEmpty(txtTimeSlot.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Timetables (SubjectID, TimeSlot, RoomID) VALUES (@subject, @time, @room)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@subject", cmbSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@time", txtTimeSlot.Text.Trim());
                    cmd.Parameters.AddWithValue("@room", cmbRoom.SelectedValue);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Timetable added.");
            LoadTimetables();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTimetableID.Text))
            {
                MessageBox.Show("Please select a timetable entry to update.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Timetables SET SubjectID = @subject, TimeSlot = @time, RoomID = @room WHERE TimetableID = @id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@subject", cmbSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@time", txtTimeSlot.Text.Trim());
                    cmd.Parameters.AddWithValue("@room", cmbRoom.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", txtTimetableID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Timetable updated.");
            LoadTimetables();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTimetableID.Text))
            {
                MessageBox.Show("Please select a timetable entry to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure to delete this entry?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Timetables WHERE TimetableID = @id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtTimetableID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Timetable deleted.");
                LoadTimetables();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvTimetables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTimetables.Rows[e.RowIndex];
                txtTimetableID.Text = row.Cells["TimetableID"].Value.ToString();
                txtTimeSlot.Text = row.Cells["TimeSlot"].Value.ToString();
                cmbSubject.Text = row.Cells["SubjectName"].Value.ToString();
                cmbRoom.Text = row.Cells["RoomName"].Value.ToString();
            }
        }
    }
}

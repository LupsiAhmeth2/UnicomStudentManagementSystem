using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using UnicomTICManagementSystem.Repositories;

namespace UnicomTICManagementSystem.Views
{
    public partial class RoomForm : Form
    {
        public RoomForm()
        {
            InitializeComponent();
            LoadRooms();
        }

        private void LoadRooms()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Rooms";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvRooms.DataSource = dt;
            }
        }

        private void ClearInputs()
        {
            txtRoomID.Text = "";
            txtRoomName.Text = "";
            txtRoomType.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomName.Text) || string.IsNullOrWhiteSpace(txtRoomType.Text))
            {
                MessageBox.Show("Please enter Room Name and Room Type.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Rooms (RoomName, RoomType) VALUES (@name, @type)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtRoomName.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", txtRoomType.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Room added successfully.");
            LoadRooms();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomID.Text))
            {
                MessageBox.Show("Please select a room to update.");
                return;
            }

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Rooms SET RoomName = @name, RoomType = @type WHERE RoomID = @id";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtRoomName.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", txtRoomType.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", txtRoomID.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Room updated successfully.");
            LoadRooms();
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomID.Text))
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this room?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Rooms WHERE RoomID = @id";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtRoomID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Room deleted.");
                LoadRooms();
                ClearInputs();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvRooms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRooms.Rows[e.RowIndex];
                txtRoomID.Text = row.Cells["RoomID"].Value.ToString();
                txtRoomName.Text = row.Cells["RoomName"].Value.ToString();
                txtRoomType.Text = row.Cells["RoomType"].Value.ToString();
            }
        }
    }
}

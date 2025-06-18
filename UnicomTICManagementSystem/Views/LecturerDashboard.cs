// ✅ LecturerDashboard.cs
using System;
using System.Windows.Forms;

namespace UnicomTICManagementSystem.Views
{
    public partial class LecturerDashboard : Form
    {
        public LecturerDashboard()
        {
            InitializeComponent();
        }

        private void btnMarks_Click(object sender, EventArgs e)
        {
            MarkForm markForm = new MarkForm();
            markForm.Show();
        }

        private void btnTimetable_Click(object sender, EventArgs e)
        {
            TimetableForm timetableForm = new TimetableForm();
            timetableForm.Show();
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attendanceForm = new AttendanceForm();
            attendanceForm.Show();
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
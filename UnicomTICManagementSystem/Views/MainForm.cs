using System;
using System.Windows.Forms;

namespace UnicomTICManagementSystem.Views
{
    public partial class MainForm : Form
    {
        private string _role;

        public MainForm(string role)
        {
            InitializeComponent();
            _role = role;
            lblWelcome.Text = $"Welcome, {_role}!";
            ApplyRolePermissions();
        }

        private void ApplyRolePermissions()
        {
            // Admin-only controls
            btnCourses.Visible = _role == "Admin";
            btnStudents.Visible = _role == "Admin";
            btnExams.Visible = _role == "Admin";
            btnMarks.Visible = _role == "Admin";
            btnSubjects.Visible = _role == "Admin";
            btnTimetable.Visible = _role == "Admin";
            btnRoom.Visible = _role == "Admin";
            btnAttendance.Visible = _role == "Admin"; // ✅ Make Attendance visible only for Admin
        }

        private void btnCourses_Click(object sender, EventArgs e) => new CourseForm().Show();
        private void btnStudents_Click(object sender, EventArgs e) => new StudentForm().Show();
        private void btnExams_Click(object sender, EventArgs e) => new ExamForm().Show();
        private void btnMarks_Click(object sender, EventArgs e) => new MarkForm().Show();
        private void btnSubjects_Click(object sender, EventArgs e) => new SubjectForm().Show();
        private void btnTimetable_Click(object sender, EventArgs e) => new TimetableForm().Show();
        private void btnRoom_Click(object sender, EventArgs e) => new RoomForm().Show();

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            // ✅ This opens the AttendanceForm
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
                new LoginForm().Show();
            }
        }
    }
}

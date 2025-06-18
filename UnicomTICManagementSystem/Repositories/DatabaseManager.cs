using System;
using System.Data.SQLite;
using System.IO;

namespace UnicomTICManagementSystem.Repositories
{
    public class DatabaseManager
    {
        private static readonly string dbFile = "unicomtic.db";
        private static readonly string connectionString = $"Data Source={dbFile};Version=3;";

        public static void InitializeDatabase()
        {
            // Step 1: Create DB file if it doesn't exist
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Step 2: Create required tables
                string[] tableCommands =
                {
                    @"CREATE TABLE IF NOT EXISTS Users (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE,
                        Password TEXT,
                        Role TEXT
                    );",

                    @"CREATE TABLE IF NOT EXISTS Courses (
                        CourseID INTEGER PRIMARY KEY AUTOINCREMENT,
                        CourseName TEXT
                    );",

                    @"CREATE TABLE IF NOT EXISTS Subjects (
                        SubjectID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SubjectName TEXT,
                        CourseID INTEGER
                    );",

                    @"CREATE TABLE IF NOT EXISTS Students (
                        StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        CourseID INTEGER
                    );",

                    @"CREATE TABLE IF NOT EXISTS Exams (
                        ExamID INTEGER PRIMARY KEY AUTOINCREMENT,
                        ExamName TEXT,
                        SubjectID INTEGER
                    );",

                    @"CREATE TABLE IF NOT EXISTS Marks (
                        MarkID INTEGER PRIMARY KEY AUTOINCREMENT,
                        StudentID INTEGER,
                        ExamID INTEGER,
                        Score INTEGER
                    );",

                    @"CREATE TABLE IF NOT EXISTS Rooms (
                        RoomID INTEGER PRIMARY KEY AUTOINCREMENT,
                        RoomName TEXT,
                        RoomType TEXT
                    );",

                    @"CREATE TABLE IF NOT EXISTS Timetables (
                        TimetableID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SubjectID INTEGER,
                        TimeSlot TEXT,
                        RoomID INTEGER
                    );"
                };

                foreach (var command in tableCommands)
                {
                    using (var cmd = new SQLiteCommand(command, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Step 3: Insert 4 default users using OR IGNORE to avoid locking issues
                string[] defaultUsers =
                {
                    "INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES ('admin', 'admin123', 'Admin')",
                    "INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES ('staff1', 'staff123', 'Staff')",
                    "INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES ('lecturer1', 'lect123', 'Lecturer')",
                    "INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES ('student1', 'stud123', 'Student')"
                };

                foreach (var userSql in defaultUsers)
                {
                    using (var cmd = new SQLiteCommand(userSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}

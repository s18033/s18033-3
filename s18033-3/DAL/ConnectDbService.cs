using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cw3.DAL
{
    public class ConnectDbService : IDbService
    {
        private static string connectionString = "Data Source=db-mssql;Initial Catalog=s18033;Integrated Security=True";
        public static List<Student> students = new List<Student>();

        static ConnectDbService()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "SELECT IndexNumber, FirstName, LastName FROM dbo.Student";
                    command.Connection = connection;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var newStudent = new Student();
                            newStudent.IndexNumber = reader["IndexNumber"].ToString();
                            newStudent.FirstName = reader["FirstName"].ToString();
                            newStudent.LastName = reader["LastName"].ToString();

                            ConnectDbService.students.Add(newStudent);
                        }
                    }
                }
            }
        }

        public List<Student> GetStudents()
        {
            return students;
        }

        public List<Enrollment> GetEnrollments(string indexNumber)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = $"SELECT * FROM dbo.Enrollment WHERE IdEnrollment = (SELECT IdEnrollment FROM dbo.Student WHERE Student.IndexNumber = '{indexNumber}');";
                    command.Connection = connection;

                    connection.Open();

                    List<Enrollment> enrollments = new List<Enrollment>();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var newEnrollment = new Enrollment();
                            newEnrollment.IdEnrollment = (int)reader["IdEnrollment"];
                            newEnrollment.Semester = (int)reader["Semester"];
                            newEnrollment.IdStudy = (int)reader["IdStudy"];
                            newEnrollment.StartDate = reader["StartDate"].ToString();

                            enrollments.Add(newEnrollment);
                        }
                    }

                    return enrollments;
                }
            }
        }
    }
}
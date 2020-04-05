using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cw3.DAL
{
    public class ConnectDbService : IDbService
    {
        private static string connectionString = "Data Source=db-mssql;Initial Catalog=s18033;Integrated Security=True";
        private static IEnumerable<Student> _students;

        static void MockDbService()
        {
            using (var client = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT FirstName FROM dbo.Student"))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            System.Diagnostics.Debug.WriteLine("{0}", reader.GetString(0));
                        }
                    }
                }
            }
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
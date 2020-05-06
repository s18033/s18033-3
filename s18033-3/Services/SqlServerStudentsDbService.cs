using Microsoft.AspNetCore.Mvc;
using s18033_3.DTOs.Responses;
using s18033_3.DTOs.Requests;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using System.Data;

namespace s18033_3.Services
{
    public class SqlServerStudentsDbService : ControllerBase, IStudentsDbService
    {
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18033;Integrated Security=True"))
            {
                using (SqlCommand command = new SqlCommand())
                {

                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    try
                    {

                        command.Connection = connection;
                        command.Transaction = transaction;

                        command.CommandText = "SELECT IndexNumber FROM dbo.Student WHERE IndexNumber=@IndexNumber";
                        command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                        var dataReader = command.ExecuteReader();

                        if (dataReader.HasRows)
                        {
                            dataReader.Close();
                            transaction.Rollback();
                            return BadRequest("Indeks studenta powinien byc unikalny.");
                        }

                        dataReader.Close();

                        command.CommandText = "SELECT IdStudy FROM dbo.Studies WHERE Name=@Name";
                        command.Parameters.AddWithValue("Name", request.Studies);

                        dataReader = command.ExecuteReader();

                        if (!dataReader.HasRows)
                        {
                            dataReader.Close();
                            transaction.Rollback();
                            return BadRequest("Zadane studia nie istnieja.");
                        }

                        int idStudy = 0;

                        while (dataReader.Read())
                        {
                            idStudy = dataReader.GetInt32(0);
                        }

                        dataReader.Close();

                        command.CommandText = "SELECT * FROM dbo.Enrollment WHERE IdStudy=@IdStudy AND Semester=1";
                        command.Parameters.AddWithValue("IdStudy", idStudy);

                        dataReader = command.ExecuteReader();
                        dataReader.Read();

                        if (!dataReader.HasRows)
                        {
                            dataReader.Close();
                            command.CommandText = "SELECT COUNT(*) FROM dbo.Enrollments";
                            int enrollmentsCount = command.ExecuteNonQuery();
                            dataReader.Close();

                            command.CommandText = "INSERT INTO dbo.Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@IdEnrollment, @IdStudy, 1,'2023-01-01');";
                            command.Parameters.AddWithValue("IdEnrollment", enrollmentsCount + 1);

                            dataReader = command.ExecuteReader();
                        }

                        int idEnrollment = dataReader.GetInt32(dataReader.GetOrdinal("IdEnrollment"));
                        DateTime startDate = dataReader.GetDateTime(dataReader.GetOrdinal("StartDate"));

                        dataReader.Close();

                        command.CommandText = "INSERT INTO dbo.Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                        command.Parameters.AddWithValue("FirstName", request.FirstName);
                        command.Parameters.AddWithValue("LastName", request.LastName);
                        command.Parameters.AddWithValue("BirthDate", request.BirthDate);
                        command.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                        command.ExecuteNonQuery();

                        transaction.Commit();

                        EnrollStudentResponse response = new EnrollStudentResponse();
                        response.Semester = 1;
                        response.StartDate = startDate;

                        return Ok(response);

                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return BadRequest("Wystapil problem z polaczeniem sie z baza.");
        }

        public IActionResult PromoteStudents(int semester, string studies)
        {
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18033;Integrated Security=True"))
            {
                using (SqlCommand command = new SqlCommand())
                {

                    connection.Open();

                    try
                    {

                        command.Connection = connection;

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.PromoteStudent";
                        command.Parameters.AddWithValue("@Studies", studies);
                        command.Parameters.AddWithValue("@Semester", semester);

                        // Procedura mimo sprawdzenia w SSMS nie jest w stanie się zawsze zwraca wyjątek (RAISERROR) i trafia do 404 (NotFound niżej). Parametry studies i semester są prawidłowe - sprawdzałem.

                        var dataReader = command.ExecuteReader();

                        var idEnrollmentDb = command.Parameters["@ReturnVal"].Value;
                        dataReader.Close();

                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT IdEnrollment, StartDate, IdStudy FROM dbo.Enrollment WHERE IdEnrollment=@IdEnrollment";
                        command.Parameters.AddWithValue("@IdEnrollment", idEnrollmentDb);

                        dataReader = command.ExecuteReader();
                        dataReader.Read();

                        int idEnrollment = dataReader.GetInt32(dataReader.GetOrdinal("IdEnrollment"));
                        int idStudy = dataReader.GetInt32(dataReader.GetOrdinal("IdStudy"));
                        DateTime startDate = dataReader.GetDateTime(dataReader.GetOrdinal("StartDate"));

                        Enrollment response = new Enrollment();
                        response.IdStudy = idStudy;
                        response.Semester = semester + 1;
                        response.IdEnrollment = idEnrollment;
                        response.StartDate = startDate.ToString();

                        return Ok(response);

                    }
                    catch (SqlException)
                    {
                        return NotFound("Nie znaleziono studiów o zadanej nazwie");
                    }
                }
            }
        }
    }
}

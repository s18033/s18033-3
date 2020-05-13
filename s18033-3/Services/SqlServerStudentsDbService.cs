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
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
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
                            throw new Exception("Indeks studenta powinien byc unikalny.");
                        }

                        dataReader.Close();

                        command.CommandText = "SELECT IdStudy FROM dbo.Studies WHERE Name=@Name";
                        command.Parameters.AddWithValue("Name", request.Studies);

                        dataReader = command.ExecuteReader();

                        if (!dataReader.HasRows)
                        {
                            dataReader.Close();
                            transaction.Rollback();
                            throw new Exception("Zadane studia nie istnieją");
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

                        return response;

                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                    }
                }
            }
            throw new Exception("Wystapił problem z połączeniem sie z bazą.");
        }

        public EnrollStudentResponse PromoteStudents(int semester, string studies)
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
                        command.Parameters.Add("@retValue", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                        // Procedura mimo sprawdzenia w SSMS nie jest w stanie się wykonać i zawsze zwraca wyjątek (RAISERROR). 
                        // Parametry studies i semester są prawidłowe - sprawdzałem. Mimo porady aby dodać linię 134 nadal rozwiązanie nie działa :(
                        // Do repozytorium dołączony jest plik z procedurą (StoredProcedure.sql)

                        // Dodatkowo nie wiem jak obsłużyć wyjątek procedury - że jest błąd po stronie serwera, jak pobrać wiadomość?

                        var dataReader = command.ExecuteReader();

                        var idEnrollmentDb = (int)command.Parameters["@retValue"].Value;
                        dataReader.Close();

                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT IdEnrollment, StartDate, IdStudy FROM dbo.Enrollment WHERE IdEnrollment=@IdEnrollment";
                        command.Parameters.AddWithValue("@IdEnrollment", idEnrollmentDb);

                        dataReader = command.ExecuteReader();
                        dataReader.Read();

                        int idEnrollment = dataReader.GetInt32(dataReader.GetOrdinal("IdEnrollment"));
                        int idStudy = dataReader.GetInt32(dataReader.GetOrdinal("IdStudy"));
                        DateTime startDate = dataReader.GetDateTime(dataReader.GetOrdinal("StartDate"));

                        var response = new EnrollStudentResponse();
                        response.Semester = semester + 1;
                        response.StartDate = startDate;

                        return response;

                    }
                    catch (SqlException)
                    {
                        throw new Exception("Nie wykonano promocji studiów o zadanej nazwie.");
                    }
                }
            }
        }
    }
}

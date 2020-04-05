using Cw3.Models;
using System.Collections.Generic;

namespace Cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static List<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{FirstName="Jan", LastName="Kowalski"},
                new Student{FirstName="Anna", LastName="Malewski"},
                new Student{FirstName="Andrzej", LastName="Andrzejewicz"}
            };
        }

        public List<Student> GetStudents()
        {
            return _students;
        }

        public List<Enrollment> GetEnrollments(string indexNumber)
        {
            return new List<Enrollment>();
        }
    }
}
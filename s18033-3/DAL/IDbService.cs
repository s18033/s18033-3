using Cw3.Models;
using System.Collections.Generic;

namespace Cw3.DAL
{
    public interface IDbService
    {
        public List<Student> GetStudents();
        public List<Enrollment> GetEnrollments(string indexNumber);
    }
}
using Microsoft.AspNetCore.Mvc;
using s18033_3.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace s18033_3.Services
{
    public interface IStudentsDbService
    {
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public void PromoteStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using s18033_3.DTOs.Requests;
using s18033_3.DTOs.Responses;
using s18033_3.Services;

namespace s18033_3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            this._service = service;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return _service.EnrollStudent(request);
        }

        [HttpPost("promotions/")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            return _service.PromoteStudents(request.Semester, request.Studies);
        }
    }
}